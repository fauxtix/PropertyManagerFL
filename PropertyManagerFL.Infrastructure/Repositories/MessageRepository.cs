using Dapper;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.DapperContext;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Core.Entities;
using System.Data;

namespace PropertyManagerFL.Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IDapperContext _context;
        private readonly ILogger<MessageRepository> _logger;

        public MessageRepository(IDapperContext context, ILogger<MessageRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<int> Add(Message message)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@DestinationEmail", message.DestinationEmail);
            parameters.Add("@SenderEmail", message.SenderEmail);
            parameters.Add("@SubjectTitle", message.SubjectTitle);
            parameters.Add("@MessageContent", message.MessageContent);
            parameters.Add("@MessageType", message.MessageType);
            parameters.Add("@MessageSentOn", message.MessageSentOn);
            parameters.Add("@MessageReceivedOn", message.MessageReceivedOn);
            parameters.Add("@TenantId", message.TenantId);
            parameters.Add("@ReferenceId", message.ReferenceId);

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    int insertedId = await connection.QueryFirstAsync<int>("usp_Messages_Insert",
                         param: parameters, commandType: CommandType.StoredProcedure);

                    return insertedId;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return -1;
            }

        }

        public async Task<bool> Delete(int Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@MessageId", Id);

                using (var connection = _context.CreateConnection())
                {
                    await connection.ExecuteAsync("usp_Messages_Delete",
                    param: parameters, commandType: CommandType.StoredProcedure);
                    return true;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }
        }

        public async Task<List<Message>> GetAllMessages()
        {
            try
            {
                _logger.LogInformation("Get todos os fiadores");

                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryAsync<Message>("usp_Messages_GetAll",
                        commandType: CommandType.StoredProcedure);
                    return output.ToList();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<Message> GetMessageById(int Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@MessageId", Id);

                using (var connection = _context.CreateConnection())
                {
                    return await connection.QueryFirstAsync<Message>("usp_Messages_GetById",
                    param: parameters, commandType: CommandType.StoredProcedure);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }

        }

        public async Task<bool> Save(Message message)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@MessageId", message.MessageId);
            parameters.Add("@DestinationEmail", message.DestinationEmail);
            parameters.Add("@SenderEmail", message.SenderEmail);
            parameters.Add("@SubjectTitle", message.SubjectTitle);
            parameters.Add("@MessageContent", message.MessageContent);
            parameters.Add("@MessageType", message.MessageType);
            parameters.Add("@MessageSentOn", message.MessageSentOn);
            parameters.Add("@MessageReceivedOn", message.MessageReceivedOn);
            parameters.Add("@TenantId", message.TenantId);
            parameters.Add("@ReferenceId", message.ReferenceId);

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    await connection.QueryFirstAsync<int>("usp_Messages_Update",
                         param: parameters, commandType: CommandType.StoredProcedure);

                    return true;

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
    }
}
