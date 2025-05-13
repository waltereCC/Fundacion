document.addEventListener('DOMContentLoaded', function () {
    var chatbotIcon = document.getElementById('chatbot-icon');
    var chatbotContainer = document.getElementById('chatbot-container');

    chatbotIcon.addEventListener('click', function () {
        if (chatbotContainer.style.display === 'none' || chatbotContainer.style.display === '') {
            chatbotContainer.style.display = 'block'; // Muestra el chatbot
        } else {
            chatbotContainer.style.display = 'none'; // Oculta el chatbot
        }
    });
});