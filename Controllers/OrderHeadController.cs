using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopPrimeVueServerAPI.Crud;
using ShopPrimeVueServerAPI.Models;
using System.Security.Claims;

namespace ShopPrimeVueServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderHeadController : ControllerBase
    {
        // GET: api/<OrderHeadController>
        [HttpGet]
        public ActionResult<IEnumerable<OrderHead>> Get()
        {
            try
            {
                IEnumerable<OrderHead> orders;

                if (User.IsInRole("admin"))
                {
                    orders = OrderHeadCrud.GetAll();
                }
                else
                {
                    // Используем стандартные ClaimTypes для большей надежности
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                   ?? User.FindFirst("id")?.Value;

                    if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                    {
                        return Unauthorized("Не удалось определить идентификатор пользователя");
                    }

                    orders = OrderHeadCrud.GetAllByUserId(userId);
                }

                return Ok(orders.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/<OrderHeadController>/5
        [HttpGet("{id}")]
        public ActionResult<OrderHead> Get(int id)
        {
            try
            {
                OrderHead model = OrderHeadCrud.GetOne(id);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/<OrderHeadController>
        [HttpPost]
        public ActionResult<OrderHead> Post(OrderHead model)
        {
            try
            {
                model = OrderHeadCrud.Insert(model);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<OrderHeadController>/5
        [HttpPut]
        public ActionResult Put(OrderHead model)
        {
            try
            {
                OrderHeadCrud.Update(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/<OrderHeadController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                OrderHeadCrud.Del(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
