using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "Admin")]

    [ApiController]
    public class Info : ControllerBase
    {

        [HttpGet]
        public IActionResult Get()
        {
            Object data = new
            {
                connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=StudentK167932;Integrated Security=True",
                integrationName = "nportal.pl/mieszkania",
                studentIndex = "s167932",
                studentName = "Oskar Kłos"
            };

            return Ok(data);
        }

    }
}
