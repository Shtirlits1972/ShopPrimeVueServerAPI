using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopPrimeVueServerAPI.Crud;
using ShopPrimeVueServerAPI.Models;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace ShopPrimeVueServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        // GET: api/<CategoryController>
        [HttpGet]
        public ActionResult<IEnumerable<Category>>Get()
        {
            try
            {
                List<Category> list = CategoryCrud.GetAll();
                return Ok(list);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        // GET api/<CategoryController>/5
        [HttpGet("{id}")]
        public ActionResult<Category> Get(int id)
        {
            try
            {
                Category model = CategoryCrud.GetOne(id);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/<CategoryController>
        [HttpPost]
        public ActionResult<Category> Post(Category model)
        {
            try
            {
                 model = CategoryCrud.Insert(model);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<CategoryController>/5
        [HttpPut]
        public ActionResult Put(Category model)
        {
            try
            {
                CategoryCrud.Update(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/<CategoryController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                CategoryCrud.Del(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
