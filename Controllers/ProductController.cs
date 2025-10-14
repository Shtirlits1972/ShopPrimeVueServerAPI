using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using ShopPrimeVueServerAPI.Crud;
using ShopPrimeVueServerAPI.Models;
using System.Threading.Tasks;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShopPrimeVueServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        string productImageDir = @"wwwroot/images/product/";


        [HttpPost]
        [Route("/api/Products/DeleteImage")]
        public async Task<IActionResult> ManageImage(int ProductId)
        {
            try
            {
                if (Directory.Exists(productImageDir + ProductId + "/")  )
                {
                    string[] fileArray = Directory.GetFiles(productImageDir + ProductId);
                    foreach (string filePath in fileArray)
                    {
                        FileInfo fi = new FileInfo(filePath);
                        fi.Delete();
                    }
                }
                else
                {
                    DirectoryInfo di = Directory.CreateDirectory(productImageDir + ProductId);
                    return Ok("Success on Create Directory");
                }
                return Ok("Success on DeleteImage");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        [HttpPost]
        [Route("/api/Products/LoadImage")]
        public async Task<IActionResult> LoadImage(IFormFile foto, [FromForm] int strId, [FromForm] bool IsEdit = false)
        {
            int h = 0;
            try
            {
                 await   ManageImage(strId);

                string finalFileName = "X"; // По умолчанию - нет изображения

                if (foto != null)
                {
                    // Генерируем уникальное имя файла чтобы избежать кэширования
                    string fileExtension = Path.GetExtension(foto.FileName);
                    finalFileName = $"{Guid.NewGuid()}{fileExtension}";
                    int gg = 0;
                    try
                    {
                        using (FileStream fileStream = new FileStream(
                            productImageDir + strId.ToString() + "/" + finalFileName,
                            FileMode.Create))
                        {
                            await foto.CopyToAsync(fileStream);
                        }

                        Product product = ProductCrud.GetOne(strId);
                        product.Foto = finalFileName;
                        ProductCrud.Update(product);
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new { error = $"Ошибка сохранения файла: {ex.Message}" });
                    }
                }

                int y = 0;
                // Возвращаем результат с именем файла
                return Ok(new
                {
                    success = true,
                    fileName = finalFileName,
                    timestamp = DateTime.Now.Ticks // Для избежания кэширования
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        // GET: api/<ProductController>
        [HttpGet]
        public ActionResult<IEnumerable<Product>> Get()
        {
            try
            {
                List<Product> list = ProductCrud.GetAll();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public ActionResult<Product> Get(int id)
        {
            try
            {
                Product model = ProductCrud.GetOne(id);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/<ProductController>
        [HttpPost]
        public async Task<ActionResult<Product>> Post(Product model)
        {
            try
            {
                model = ProductCrud.Insert(model);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<ProductController>/5
        [HttpPut]
        public ActionResult Put(Product model)
        {
            try
            {
                ProductCrud.Update(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                ProductCrud.Del(id);
                Directory.Delete(productImageDir + id, true);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
