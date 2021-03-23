using System;

namespace Stratego.Domain
{
    //DO NOT TOUCH THIS FILE!
    //This file will make sure there is a Friendships table in the database which is needed for an extra.
    public class Friendship
    {
        public User Friend1 { get; set; }
        public Guid Friend1Id { get; set; }
        public bool ConfirmedByFriend1 { get; set; }

        public User Friend2 { get; set; }
        public Guid Friend2Id { get; set; }
        public bool ConfirmedByFriend2 { get; set; }
    }
}