﻿using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using MyWebAPI.Security;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace MyWebAPI.Controllers
{
	[Route("api/[controller]")]
	public class AuthController : Controller
	{
		public IConfiguration Configuration { get; }

		public AuthController(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult Login([FromBody] LoginInputModel model)
		{
			if (!ModelState.IsValid)
			{
				return Unauthorized("Bad credentials");
			}
			var result = Microsoft.AspNetCore.Identity.SignInResult.Failed;

			if (model.UserName == "admin")
			{
				result = Microsoft.AspNetCore.Identity.SignInResult.Success;

				return new OkObjectResult(GetToken());
			}

			return Unauthorized();
		}

		[Authorize]
		[HttpGet("/claims")]
		public ActionResult<IEnumerable<string>> Get()
		{
			return new JsonResult(User.Claims.Select(c => new { c.Type, c.Value }));
		}

		internal string GetToken()
		{
			var token = JwtTokenBuilder.GetSecuredToken(Configuration);

			var result = new JwtSecurityTokenHandler().WriteToken(token);

			return result;
		}
	}
}