using Guts.Client.Shared;
using NUnit.Framework;
using Stratego.Domain.ArmyDomain;
using Stratego.Domain.ArmyDomain.Contracts;
using Stratego.TestTools.Builders;
using System;
using System.Collections.Generic;
using Stratego.TestTools;

namespace Stratego.Domain.Tests
{
    public class SpecialPieceTests
    {
        [MonitoredTest("Bomb and Flag should not be moveable")]
        public void BombAndFlagShouldNotBeMoveable()
        {
            Bomb bomb = new Bomb();
            Assert.IsNotInstanceOf<MoveablePiece>(bomb, "The Bomb class should not inherit from the MoveablePiece class");
            Assert.That(bomb.IsMoveable, Is.False, "The 'IsMoveable' property of a bomb should be false");

            Flag flag = new Flag();
            Assert.IsNotInstanceOf<MoveablePiece>(flag, "The Flag class should not inherit from the MoveablePiece class");
            Assert.That(flag.IsMoveable, Is.False, "The 'IsMoveable' property of a flag should be false");
        }

        [MonitoredTest("Attack - Should throw ApplicationException when a flag tries to attack")]
        public void Attack_ShouldThrowApplicationExceptionWhenTheFlagTriesToAttack()
        {
            //Arrange
            IPiece flag = new Flag();
            IPiece opponent = new PieceMockBuilder().Mock.Object;

            //Act + Assert
            Assert.That(() => flag.Attack(opponent),
                Throws.InstanceOf<ApplicationException>().With.Message.Not.Empty,
                "No ApplicationException is thrown or the exception has an empty message");
        }

        [MonitoredTest("Attack - Should throw ApplicationException when a bomb tries to attack")]
        public void Attack_ShouldThrowApplicationExceptionWhenABombTriesToAttack()
        {
            //Arrange
            IPiece bomb = new Bomb();
            IPiece opponent = new PieceMockBuilder().Mock.Object;

            //Act + Assert
            Assert.That(() => bomb.Attack(opponent),
                Throws.InstanceOf<ApplicationException>().With.Message.Not.Empty,
                "No ApplicationException is thrown or the exception has an empty message");
        }

        [MonitoredTest("Attack - The Marshal should die when attacked by a Spy.")]
        public void Attack_MarshalShouldDieWhenAttackedByASpy()
        {
            //Arrange
            IPiece marshal = new Marshal();
            IPiece spy = new Spy();
            spy.Attack(marshal);

            Assert.That(marshal.IsAlive, Is.False, "The marshal should die when attacked by a spy");
            Assert.That(spy.IsAlive, Is.True, "The spy should stay alive when it attacks a marshal");
        }

        [MonitoredTest("Attack - The Spy should die when attacking a non-Marshal.")]
        public void Attack_SpyShouldDieWhenAttackingANonMarshal()
        {
            //Arrange
            IPiece moveablePiece = new List<IPiece> {new Spy(), new Scout(), new Sergeant(), new General()}
                .NextRandomElement();
            IPiece spy = new Spy();

            //Act
            spy.Attack(moveablePiece);

            //Assert
            Assert.That(spy.IsAlive, Is.False, $"The spy should die when attacking a {moveablePiece.Name}");
        }

        [MonitoredTest("Attack - The bomb should die when attacked by a miner.")]
        public void Attack_BombShouldDieWhenAttackedByAMiner()
        {
            //Arrange
            IPiece bomb = new Bomb();
            IPiece miner = new Miner();
            
            //Act
            miner.Attack(bomb);

            //Assert
            Assert.That(bomb.IsAlive, Is.False, "The bomb should die when attacked by a miner");
            Assert.That(miner.IsAlive, Is.True, "The miner should stay alive when it attacks a bomb");
        }

        [MonitoredTest("Attack - The miner should die when attacking a non-Bomb with higher strength.")]
        public void Attack_MinerShouldDieWhenAttackingANonBombWithHigherStrength()
        {
            //Arrange
            IPiece strongerPiece = new List<IPiece> { new Sergeant(), new General() }.NextRandomElement();
            IPiece miner = new Miner();

            //Act
            miner.Attack(strongerPiece);

            //Assert
            Assert.That(miner.IsAlive, Is.False, $"The miner should die when attacking a {strongerPiece.Name}");
        }

        [MonitoredTest("Constructor - a Scout can move any distance")]
        public void AScoutCanMoveAnyDistance()
        {
            //Arrange
            IPiece scout = new Scout();
 
            //Assert
            Assert.That(scout.CanMoveAnyDistance, Is.True);
        }
    }
}
