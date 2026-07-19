using System.Security.Claims;
using Lumora.Application.Interfaces.Services;
using Lumora.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lumora.Web.Controllers;

[Authorize(Roles = RoleConstants.Student)]
public class ChatController : Controller
{
    private readonly IChatService _chat;

    public ChatController(IChatService chat)
    {
        _chat = chat;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var conversationId = await _chat.GetOrCreateConversationIdAsync(studentId);
        await _chat.MarkReadAsync(conversationId, readerIsAdmin: false);

        ViewBag.ConversationId = conversationId;
        var messages = await _chat.GetMessagesAsync(conversationId);
        return View(messages);
    }
}
