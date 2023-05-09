using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OrderoApi.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Nodes;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OrderoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private IConfiguration _configuration;

        [HttpGet]
        public JsonResult Get()
        {
            return new JsonResult("{\r\n\"id\": <this is the Employee ID referenced in Path parameter>,\r\n\"fictive\": <boolean>,\r\n\"employeeId\": <same as the id above, where the above element is left for\r\nlegacy purposes – use this for element instead>,\r\n\"wageCode\": \"<string for wage code for the employee>\",\r\n\"cardNumber\": \"<employees entry/id-card number>\",\r\n\"departmentId\": <primary department>,\r\n\"name\": \"<name of employee>\",\r\n\"email\": \"<employees email>\",\r\n\"jobFunctionId\": <integer>,\r\n\"jobPositionId\": <integer>,\r\n\"mobilePhone\": \"<mobile phone number excluding country prefix>\",\r\n\"ssn\": \"<social security number or equivalent id>\",\r\n\"initials\": \"<employee initials>\",\r\n\"phone\": \"<phone number without country prefix>\",\r\n\"normHours\": <number of hours>,\r\n\"seniority\": \"<ISO8106 date>\",\r\n\"address\": \"<street name and number>\",\r\n\"postalCode\": \"<postal code>\",\r\n\"city\": \"<city name>\",\r\n\"birthdate\": null or \"<ISO8601 date>\",\r\n\"nationality\": \"<ISO 3166-1 alpha-2 country code>\",\r\n\"gender\": \"<currently only the following values are valid\r\n(not case-sensitive):\r\n'Male','Female', 'Undisclosed' or 'M', 'F', 'U' >\",");
           // return "{\r\n \"id\": <this is the Employee ID referenced in Path parameter>,\r\n\"fictive\": <boolean>,\r\n\"employeeId\": <same as the id above, where the above element is left for\r\nlegacy purposes – use this for element instead>,\r\n\"wageCode\": \"<string for wage code for the employee>\",\r\n \"cardNumber\": \"<employees entry/id-card number>\",\r\n \"departmentId\": <primary department>,\r\n\"name\": \"<name of employee>\",\r\n\"email\": \"<employees email>\",\r\n\"jobFunctionId\": <integer>,\r\n \"jobPositionId\": <integer>,\r\n \"mobilePhone\": \"<mobile phone number excluding country prefix>\",\r\n \"ssn\": \"<social security number or equivalent id>\",\r\n \"initials\": \"<employee initials>\",\r\n \"phone\": \"<phone number without country prefix>\",\r\n \"normHours\": <number of hours>,\r\n \"seniority\": \"<ISO8106 date>\",\r\n \"address\": \"<street name and number>\",\r\n \"postalCode\": \"<postal code>\",\r\n \"city\": \"<city name>\",\r\n \"birthdate\": null or \"<ISO8601 date>\",\r\n\"nationality\": \"<ISO 3166-1 alpha-2 country code>\",\r\n \"gender\": \"<currently only the following values are valid\r\n(not case-sensitive):\r\n'Male','Female', 'Undisclosed' or 'M', 'F', 'U' >\",\"employmentStart\": \"<date string for first day of employment>\",\r\n \"employmentEnd\": \"<date in a valid ISO8601 format>\" or null,\r\n \"lastWorkingDate\": \"<date in a valid ISO8601 format>\" or null,\r\n \"employmentType\": \"<how the employee is employeed - only one value –\r\n'etMonthlyPaidFulltime',\r\n'etMonthlyPaidParttime',\r\n'etHourlyPaidWithNormHours' and\r\n'etHourlyPaidWithoutNormHours'>\",\r\n \"phonePrefix\": \"<country prefix for phone e.g. 45 for Denmark>\",\r\n \"mobilePrefix\": \"<country prefix for mobilephone>\",\r\n \"noticeCode\": null or JSON object{\r\n\"code\": \"<Code for termination reason>\",\r\n \"text\": \"<name for termination reason>\",\r\n},\r\n \"secondaryDepartmentList\": [\r\n \"<string containing department number>\"\r\n <may also just be an empty array [ ] >\r\n ],\r\n \"secondaryJobFunctionList\": [\r\n\"<string containing externalID of other job function>\"\r\n<may also just be an empty array [ ] >\r\n ],\r\n\"salary\": {\r\n\"rate\": \"<salary rate>\",\r\n\"methodOfPay\": \"<method of pay : 'hourly' or 'monthly>'\"\r\n<notice that the salary element is only included if the API user has\r\npermission to view salary – otherwise it is omitted>\r\n},\r\n \"agreementName\": \"<name of agreement>\",\r\n \"agreementId\": \"<id of agreement>\r\n}"
        }

        public class AuthenticationRequestBody
        {
            public string? UserName { get; set; }
            public string? Password { get; set; }
        }


        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }



        // POST api/<AuthenticationController>
        //URI'en gør vi til localhost:port//api/authentication/authenticate
        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate(AuthenticationRequestBody authReqBody)
        {
            //Validér bruger
            var user = ValidateUserCredentials(authReqBody.UserName, authReqBody.Password);
            if (user == null)
            {
                return Unauthorized();
            }

            //Skab en token
            var securityKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(_configuration["Authentication:SecretForKey"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            // Skabe dine claims. Claims er bare KeyValue pairs der beskriver rettigheder :)
            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("name", user.Name));
            claimsForToken.Add(new Claim("sikkerhedsniveau", "y.Hem"));

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            var tokenToReturn = new JwtSecurityTokenHandler()
                .WriteToken(jwtSecurityToken);
            return Ok(tokenToReturn);


        }



        private User ValidateUserCredentials(string? userName, string? password)
        {

            // Check i database om brugeren eksisterer
            return new User() { Name = "Hubert Nielsen" };

        }


    }
}
