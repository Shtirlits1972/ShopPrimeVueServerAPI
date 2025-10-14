using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopPrimeVueServerAPI.Crud;
using ShopPrimeVueServerAPI.Models;

namespace ShopPrimeVueServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderDetailController : ControllerBase
    {
        // GET: api/<OrderDetailController>
        [HttpGet("{orderId}")]
        public ActionResult<IEnumerable<OrderDetail>> GetALL(int orderId)
        {
            try
            {
                List<OrderDetail> list = OrderDetailCrud.GetAll(orderId);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/<OrderDetailController>/5
        [HttpGet]
        public ActionResult<OrderDetail> GetOne([FromQuery] int  id)
        {
            try
            {
                OrderDetail model = OrderDetailCrud.GetOne(id);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/<OrderDetailController>
        [HttpPost]
        public ActionResult<OrderDetail> Post(OrderDetail model)
        {
            try
            {
                model = OrderDetailCrud.Insert(model);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<OrderDetailController>/5
        [HttpPut]
        public ActionResult Put(OrderDetail model)
        {
            try
            {
                OrderDetailCrud.Update(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/<OrderDetailController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                OrderDetailCrud.Del(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
