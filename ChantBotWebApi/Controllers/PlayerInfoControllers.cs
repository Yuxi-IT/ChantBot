using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using ChantBot.Commands;
using ChantBot.Types;

namespace ChantBotWebApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        [HttpGet("info")]
        public IActionResult GetPlayerInfo([FromQuery] string id)
        {
            if (!PlayerType.PlayerExists(id))
            {
                var player = new Player
                {
                    ID = id,
                    Amount = 100,
                    Level = 1,
                    VIPLevel = 0,
                    Invites = new string[] { },
                    PaymentCode = "",
                    LastCheckIn = DateTime.Now.ToString("yyyyMMdd"),
                    RegDate = DateTime.Now.ToString("yyyyMMdd"),
                    InviteCode = PlayerType.GenerateUuid()
                };

                PlayerType.AddPlayer(player);
                return Ok(new { code = 200, data = player });
            }
            else
            {
                var player = PlayerType.GetPlayerById(id);
                return Ok(new { code = 200, data = player });
            }
        }

        [HttpGet("checkin")]
        public async Task<IActionResult> DailyCheckIn([FromQuery] string id)
        {
            if (!PlayerType.PlayerExists(id))
            {
                return Unauthorized(new { code = 403, message = "È¨ÏÞ²»¹»" });
            }
            else
            {
                var result = await PlayerType.CheckIn(id);
                return Ok(new { code = 200, data = result });
            }
        }
    }
}
