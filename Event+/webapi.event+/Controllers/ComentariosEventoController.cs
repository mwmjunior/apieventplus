using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.ContentModerator;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using webapi.event_.Domains;
using webapi.event_.Repositories;

namespace webapi.event_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ComentariosEventoController : ControllerBase
    {
        ComentariosEventoRepository comentario = new ComentariosEventoRepository();

        // armazena dados do servico da api externa (IA - Azure)
        private readonly ContentModeratorClient _contentModeratorClient;

        /// <summary>
        /// Construtor que recebe os dados necessarios para acesso sao servico externo 
        /// </summary>
        /// <param name="contentModeratorClient">objeto do tipo ContentModeratorClient</param>
        public ComentariosEventoController(ContentModeratorClient contentModeratorClient)
        {
            _contentModeratorClient= contentModeratorClient;
        }


        [HttpPost("ComentarioIA")]

        public async Task <IActionResult> PostIa(ComentariosEvento novocomentario)

        {
            try
            {
                //if(comentario.Descricao !=null) || comentario.Descricao =="")
                //if (string.IsNullOrEmpty(comentario.Descricao))
                //if((comentario.Descricao) .IsNullOrEmpty)

                if(string.IsNullOrEmpty(novocomentario.Descricao))
                {
                    return BadRequest("A descricao do comenterio nao pode estar vazio ou nulo!");
                }

                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(novocomentario.Descricao));

                var moderationResult = await _contentModeratorClient.TextModeration.ScreenTextAsync("text/plain", stream, "por", false,false, null, true);
                
                 if (moderationResult.Terms != null) 
                {
                    novocomentario.Exibe = false;

                    comentario.Cadastrar(novocomentario);
                 
                }

                 else
                {

                    novocomentario.Exibe = true;
                    comentario.Cadastrar(novocomentario);
                }
                 return StatusCode(201,novocomentario);

            }
            catch (Exception error)
            {

                return BadRequest(error.Message);    
            }
        }


        [HttpGet("ListarSomenteExibe{id}")]
        public IActionResult GetShow(Guid id)
        {
            try
            {
                return Ok(comentario.ListarSomenteExibe(id));
            }

            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(comentario.Listar());
            }

            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("BuscarPorIdUsuario")]
        public IActionResult GetByIdUser(Guid idUsuario, Guid idEvento)
        {
            try
            {
                return Ok(comentario.BuscarPorIdUsuario(idUsuario,idEvento));
            }

            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]

        public IActionResult Post(ComentariosEvento novoComentario)
        {
            try
            {
                comentario.Cadastrar(novoComentario);

                return StatusCode(201, novoComentario);
            }

            catch (Exception e)
            {
                return BadRequest(e.Message);
                throw;
            }
        }

        [HttpDelete("{id}")]
             public IActionResult Delete(Guid id)
        {
            try
            {
                comentario.Deletar(id);

                return StatusCode(204);
            }

            catch (Exception e)
            {
                return BadRequest(e.Message);
                throw;
            }
        }



    }
}
