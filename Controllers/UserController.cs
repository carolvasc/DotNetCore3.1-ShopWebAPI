using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Shop.Data;
using Shop.Models;
using Shop.Services;

namespace Shop.Controllers
{
  [Route("users")]
  public class UserController : Controller
  {
    [HttpPost]
    [Route("")]
    [AllowAnonymous]
    public async Task<ActionResult<User>> Post(
      [FromServices] DataContext context,
      [FromBody] User model
    )
    {
      // Verifica se os dados são válidos
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      try
      {
        context.Users.Add(model);
        await context.SaveChangesAsync();

        return model;
      }
      catch (Exception)
      {
        return BadRequest(new { message = "Não foi possível criar o usuário." });
      }
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<dynamic>> Authenticate(
      [FromServices] DataContext context,
      [FromBody] User model
    )
    {
      var user = await context.Users.AsNoTracking().Where(u => u.Username == model.Username && u.Password == model.Password).FirstOrDefaultAsync();

      if (user == null)
        return NotFound(new { message = "Usuário ou senha inválidos." });

      var token = TokenService.GenerateToken(user);

      return new
      {
        user = user,
        token = token
      };
    }
  }
}