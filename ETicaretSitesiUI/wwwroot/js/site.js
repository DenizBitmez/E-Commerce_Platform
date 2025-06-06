// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.getElementById('chatIcon').addEventListener('click', () => {
    const chatWindow = document.getElementById('chatWindow');
    if (chatWindow.style.display === 'none' || !chatWindow.style.display) {
        chatWindow.style.display = 'block';

        if (!window.webChatInitialized) {
            window.webChatInitialized = true;

            const directLine = window.WebChat.createDirectLine({
                secret: '6S019SjWV2hAhP5SMfMXQFrfCfYu9yKSY1BTuswMz938AABWhj1PJQQJ99BFAC77bzfAArohAAABAZBSt3wG.EWvCMsso98Wl7C3XC4oj9SLXk3XkSUgfITYXCNPdlHpyQVP52xOoJQQJ99BFAC77bzfAArohAAABAZBSqI7F' 
            });

            window.WebChat.renderWebChat({
                directLine: directLine,
                locale: 'tr-TR'
            }, document.getElementById('webchat'));
        }
    } else {
        chatWindow.style.display = 'none';
    }
});
