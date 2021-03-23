using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Stratego.Api.Models;
using Stratego.AppLogic.Contracts;
using Stratego.Common;
using Stratego.Domain;

namespace Stratego.Api.Controllers
{
    /// <summary>
    /// Provides functionality for managing friends of a user
    /// </summary>
    /// <remarks>This controller is needed for an EXTRA. Not needed to implement the minimal requirements.</remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class FriendshipsController : ApiControllerBase
    {
        private readonly IFriendshipService _friendshipService;
        private readonly IMapper _mapper;

        public FriendshipsController(IFriendshipService friendshipService, IMapper mapper)
        {
            _friendshipService = friendshipService;
            _mapper = mapper;
        }

        /// <summary>
        /// A list of users that confirmed to be the friend of the logged in user.
        /// </summary>
        [HttpGet("my-friends")]
        [ProducesResponseType(typeof(IList<UserModel>),StatusCodes.Status200OK)]
        public IActionResult GetMyFriends()
        {
            IList<User> friends = _friendshipService.GetFriends(UserId);
            IList<UserModel> friendModels = friends.Select(u => _mapper.Map<UserModel>(u)).ToList();
            return Ok(friendModels);
        }

        /// <summary>
        /// A list of users that requested (proposed) to be the friend of the logged in user.
        /// </summary>
        [HttpGet("my-friend-requests")]
        [ProducesResponseType(typeof(IList<UserModel>), StatusCodes.Status200OK)]
        public IActionResult GetMyFriendRequests()
        {
            IList<User> friends = _friendshipService.GetFriendRequests(UserId);
            IList<UserModel> friendModels = friends.Select(u => _mapper.Map<UserModel>(u)).ToList();
            return Ok(friendModels);
        }

        /// <summary>
        /// Request of the logged in user to be friends with another user
        /// </summary>
        [HttpPost("request/{targetUserId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult RequestFriendship(Guid targetUserId)
        {
            Result result = _friendshipService.DoFriendRequest(UserId, targetUserId);
            if (result.IsFailure)
            {
                return BadRequest(new ErrorModel(result.Message));
            }
            return Ok();
        }

        /// <summary>
        /// Confirm the friendship request from another user for the logged in user
        /// </summary>
        [HttpPost("confirm/{targetUserId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult ConfirmFriendship(Guid targetUserId)
        {
            Result result = _friendshipService.ConfirmFriendship(UserId, targetUserId);
            if (result.IsFailure)
            {
                return BadRequest(new ErrorModel(result.Message));
            }
            return Ok();
        }

        /// <summary>
        /// Remove the friendship between the logged in user and another user
        /// </summary>
        [HttpPost("remove/{targetUserId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult RemoveFriendship(Guid targetUserId)
        {
            Result result = _friendshipService.RemoveFriendship(UserId, targetUserId);
            if (result.IsFailure)
            {
                return BadRequest(new ErrorModel(result.Message));
            }
            return Ok();
        }
    }
}
