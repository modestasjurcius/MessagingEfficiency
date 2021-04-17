﻿using ResultsMicroservice.Entities;

namespace ResultsMicroservice.Services
{
    public interface IResultsService
    {
        ServiceResponse InsertRabbitResult(RabbitTestResult result);
        ServiceResponse UpdateRabbitResult(RabbitTestLastReceived args);
    }
}