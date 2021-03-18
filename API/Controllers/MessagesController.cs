using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }

        //to create message we'll use this 
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            //get username
            var username = User.GetUsername();
            //if username is == to recipient name in createMessageDto
            if (username == createMessageDto?.RecepientUsername.ToLower())
                return BadRequest("You cannot send messages to yourself");

            //get hold of both users who messaging
            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecepientUsername);

            if (recipient == null) return NotFound();

            //create new message

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecepientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery]
        MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();

            var messages = await _messageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize,
                messages.TotalCount, messages.TotalPages);

            return messages;
        }


        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageTgread(string username)
        {
            var currentUsername = User.GetUsername();

            return Ok(await _messageRepository.GetMessageThread(currentUsername, username));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            //hold user
            var username = User.GetUsername();

            var message = await _messageRepository.GetMesaage(id);

            //this message has nothing to do with that user
            if (message.Sender.UserName != username && message.Recipient.UserName != username)
                return Unauthorized();

            if (message.Sender.UserName == username) message.SenderDeleted = true;
            if (message.Recipient.UserName == username) message.RecipientDeleted = true;

            //now check if bot of them delete message, then delete it from server
            if(message.SenderDeleted && message.RecipientDeleted) 
                _messageRepository.DeleteMessage(message);

            if (await _messageRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleteing the message");
        }
    }
}