using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopPrimeVueServerAPI.Crud;
using ShopPrimeVueServerAPI.Models;
using System.Reflection;
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
                    var userIdClaim =  User.FindFirst("id")?.Value;


                    int hh = 0;

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
                if (id == 0)
                {
                    string newOrderNumber = OrderHeadCrud.GetNewOrderNumber();
                    DateTime currentDate = DateTime.Now.Date;

                    var userIdClaim = User.FindFirst("id")?.Value;

                    int userId = 0;
                    if (!string.IsNullOrEmpty(userIdClaim))
                    {
                        // Проверяем результат TryParse и используем существующую переменную userId
                        if (!int.TryParse(userIdClaim, out userId))
                        {
                            return Unauthorized("Не удалось определить идентификатор пользователя");
                        }
                    }

                    Users users = UsersCrud.GetOne(userId);

                    int g = 5;
                    OrderHead newOrderHead = new OrderHead
                    {
                        id = 0,
                        UserId = userId,
                        UsersName = users?.UsersName ?? "Unknown",
                        OrderNumber = newOrderNumber,
                        OrderData = currentDate,
                        TotalPrice = 0
                    };

                    return Ok(newOrderHead);
                }
                else 
                {
                    OrderHead model = OrderHeadCrud.GetOne(id);
                    return Ok(model);
                }
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
