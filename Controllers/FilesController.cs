using ApiFiles.Context;
using ApiFiles.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiFiles.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        public readonly AppDbContext context;
        public FilesController(AppDbContext context)
        {
            this.context = context;
        }

        // GET: api/<FilesController>
        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                return Ok(context.file.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/<FilesController>
        [HttpPost]
        public ActionResult PostFiles([FromForm] List<IFormFile> files)     //se cambió FromBody por FromForm porque sino tiraba error 415 media no soportada
        {
            List<Models.File> f = new List<Models.File>();

            try
            {
                if(files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        var FilePath = "D:\\Documentos\\Proyectos\\c#\\ApiFiles\\Files\\" + file.FileName;

                        //guardo en la carpeta
                        using (var stream = System.IO.File.Create(FilePath))
                        {
                            file.CopyToAsync(stream);
                        }

                        //calculo tamaño en MB
                        double tamanio = file.Length;
                        tamanio = tamanio / 1000000;    //convierto a mb
                        tamanio = Math.Round(tamanio, 2);   //redondeo a dos decimales

                        //propiedades del modelo para insertar en db
                        Models.File ff = new Models.File();
                        ff.extension = Path.GetExtension(file.FileName).Substring(1);   //obtengo le extension y quito el punto inicial para no almacenarlo en la db
                        ff.nombre = Path.GetFileNameWithoutExtension(file.FileName);
                        ff.tamanio = tamanio;
                        ff.ubicacion = FilePath;

                        //inserto en la lista
                        f.Add(ff);
                    }
                    //inserto en la db
                    context.file.AddRange(f);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(f);
        }
    }
}
