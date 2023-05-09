using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace OrderoApi.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class OrderController : ControllerBase
    {
        // GET: api/<OrderController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
            
        }

        // GET api/<OrderController>/5
        [HttpGet("{id}")]

        public ActionResult<string> Get(int id)
        {
            var sikkerhedsniveau = User.Claims.FirstOrDefault(c => c.Type == "sikkerhedsniveau")?.Value;
            if (sikkerhedsniveau == "y.hem")
            {
            return "Yderst hemmelig value :D";
            }
            return Forbid("Nix");
        }

        // POST api/<OrderController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<OrderController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<OrderController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
