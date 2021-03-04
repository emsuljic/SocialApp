using System;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly DataContext _context;
        public BuggyController(DataContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return "secret text";
        }

        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
        {
            //look for smth that we sure that not exists
            var thing = _context.Users.Find(-1);
            //
            if (thing == null) return NotFound();
            //u slucaju da se pronadje nesto
            return Ok(thing);
        }

        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {
                var thing = _context.Users.Find(-1);
                var thingToReturn = thing.ToString(); //null returned
                                                      //when use find method, if entity is found it's attached to the context and returned
                                                      // ,if no entity is found, then null is returned
                return thingToReturn;
        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("This was not a good request");
        }
    }
}