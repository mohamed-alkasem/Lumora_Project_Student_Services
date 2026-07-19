using Lumora.Application.DTOs.Chat;
using Lumora.Application.Interfaces.Services;
using Lumora.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lumora.Web.Areas.Admin.Controllers;

public class ChatController : AdminBaseController
{
    private readonly IChatService _chat;
    private readonly UserManager<ApplicationUser> _userManager;

    public ChatController(IChatService chat, UserManager<ApplicationUser> userManager)
    {
        _chat = chat;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var conversations = await _chat.GetConversationsForAdminAsync();

        // Resolve student display names (Identity lives outside Application, so we enrich here).
        var names = new Dictionary<int, string>();
        foreach (var conversation in conversations)
        {
            var user = await _userManager.FindByIdAsync(conversation.StudentUserId.ToString());
            names[conversation.StudentUserId] = user?.FullName ?? user?.Email ?? $"#{conversation.StudentUserId}";
        }

        ViewBag.StudentNames = names;
        return View(conversations);
    }

    public async Task<IActionResult> Conversation(int id)
    {
        var studentId = await _chat.GetConversationStudentIdAsync(id);
        if (studentId is null)
        {
            return NotFound();
        }

        await _chat.MarkReadAsync(id, readerIsAdmin: true);

        var student = await _userManager.FindByIdAsync(studentId.Value.ToString());
        ViewBag.ConversationId = id;
        ViewBag.StudentName = student?.FullName ?? student?.Email ?? $"#{studentId}";

        var messages = await _chat.GetMessagesAsync(id);
        return View(messages);
    }
}
