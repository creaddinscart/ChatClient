Release 1.2.7 - Automated fixes and publish

Summary:
- Built and published ChatClient v1.2.7 to publish\\1.2.7\\ChatClient.exe
- Applied automated, low-risk fixes before publish:
  - Ensured server owner receives full ChatPacket (attachments, metadata) instead of plain text
  - Moved online user count display from Home to Chat panel header
  - Added session chat history and "Save chat" button to export full conversation
  - Always send typing/typing-stop regardless of password mode; stop on send/clear
  - Bound ContextMenu per message element so owner can always access moderation menu
  - Implemented SimpleCommand.CanExecuteChanged accessors to remove warning

Notes and remaining items:
- Static analysis and build succeeded with no errors; one previous nonblocking warning was removed.
- Runtime/behavioral issues that require interactive testing and multi-client scenarios were NOT fully validated here (e.g., edge cases for typing race conditions, IP ban propagation in NAT scenarios).

Recommended manual tests:
1. Start server (owner) and join with a second client; verify owner can right-click any message and see Ban/Download options.
2. Upload an attachment from client; owner should preview and download it.
3. Verify typing indicator shows to other clients but not the typing client; test with and without server password.
4. Use Save chat to export full session history and confirm contents include system messages and attachments.

Published file:
 - publish\\1.2.7\\ChatClient.exe

If further bugs are found during interactive testing, please report specific repro steps and logs so they can be fixed in a follow-up patch.

