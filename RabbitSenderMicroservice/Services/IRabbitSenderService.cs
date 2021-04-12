using RabbitSenderMicroservice.Entities;

namespace RabbitSenderMicroservice.Services
{
    public interface IRabbitSenderService
    {
        ServiceResponse Get();
        ServiceResponse SendMessages(SendMessagesArgs args);
    }
}