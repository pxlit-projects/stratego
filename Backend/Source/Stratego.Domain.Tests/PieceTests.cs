using Guts.Client.Shared;
using NUnit.Framework;
using Stratego.Domain.ArmyDomain;
using Stratego.Domain.ArmyDomain.Contracts;
using Stratego.TestTools.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using Guts.Client.Core;

namespace Stratego.Domain.Tests
{
    [ProjectComponentTestFixture("1TINProject", "Stratego", "Piece",
        @"Stratego.Domain\ArmyDomain\Piece.cs;
Stratego.Domain\ArmyDomain\MovablePiece.cs;
Stratego.Domain\ArmyDomain\Sergeant.cs;
Stratego.Domain\ArmyDomain\Lieutenant.cs;
Stratego.Domain\ArmyDomain\Captain.cs;
Stratego.Domain\ArmyDomain\Major.cs;
Stratego.Domain\ArmyDomain\Colonel.cs;
Stratego.Domain\ArmyDomain\General.cs;
Stratego.Domain\ArmyDomain\Marshal.cs")]
    public class PieceTests
    {
        private static readonly Random Random = new Random();

        private Bomb _bomb;
        private Captain _captain;
        private Colonel _colonel;
        private Flag _flag;
        private General _general;
        private Lieutenant _lieutenant;
        private Major _major;
        private Marshal _marshal;
        private Miner _miner;
        private Scout _scout;
        private Sergeant _sergeant;
        private Spy _spy;
        private List<IPiece> _normalPieces;

        [SetUp]
        public void Setup()
        {
            _spy = new Spy();
            _scout = new Scout();
            _sergeant = new Sergeant();
            _miner = new Miner();
            _lieutenant = new Lieutenant();
            _captain = new Captain();
            _major = new Major();
            _colonel = new Colonel();
            _general = new General();
            _marshal = new Marshal();
            _flag = new Flag();
            _bomb = new Bomb();

            _normalPieces = new List<IPiece>
            {
                _sergeant,
                _lieutenant,
                _captain,
                _major,
                _colonel,
                _general,
                _marshal
            };
        }

        [MonitoredTest("Constructor - Should initialize piece correctly")]
        public void Constructor_ShouldInitializePieceCorrectly()
        {
            Assert.That(_bomb.Name, Is.EqualTo("Bomb"), "The bomb should have the Name property with value 'Bomb'");
            Assert.That(_bomb.Strength, Is.EqualTo(11), "The bomb should have the Strength property with value 11");

            Assert.That(_captain.Name, Is.EqualTo("Captain"), "The captain should have the Name property with value 'Captain'");
            Assert.That(_captain.Strength, Is.EqualTo(6), "The captain should have the Strength property with value 6");

            Assert.That(_colonel.Name, Is.EqualTo("Colonel"), "The colonel should have the Name property with value 'Colonel'");
            Assert.That(_colonel.Strength, Is.EqualTo(8), "The colonel should have the Strength property with value 8");

            Assert.That(_flag.Name, Is.EqualTo("Flag"), "The flag should have the Name property with value 'Flag'");
            Assert.That(_flag.Strength, Is.EqualTo(0), "The flag should have the Strength property with value 0");

            Assert.That(_general.Name, Is.EqualTo("General"), "The general should have the Name property with value 'General'");
            Assert.That(_general.Strength, Is.EqualTo(9), "The general should have the Strength property with value 9");

            Assert.That(_lieutenant.Name, Is.EqualTo("Lieutenant"), "The lieutenant should have the Name property with value 'Lieutenant'");
            Assert.That(_lieutenant.Strength, Is.EqualTo(5), "The lieutenant should have the Strength property with value 5");

            Assert.That(_major.Name, Is.EqualTo("Major"), "The major should have the Name property with value 'Major'");
            Assert.That(_major.Strength, Is.EqualTo(7), "The major should have the Strength property with value 7");

            Assert.That(_marshal.Name, Is.EqualTo("Marshal"), "The marshal should have the Name property with value 'Marshal'");
            Assert.That(_marshal.Strength, Is.EqualTo(10), "The marshal should have the Strength property with value 10");

            Assert.That(_miner.Name, Is.EqualTo("Miner"), "The miner should have the Name property with value 'Miner'");
            Assert.That(_miner.Strength, Is.EqualTo(3), "The miner should have the Strength property with value 3");

            Assert.That(_normalPieces, Has.All.Matches((IPiece p) => p.CanMoveAnyDistance == false),
                "All 'normal' pieces should have 'CanMoveAnyDistanceProperty' set to false.");

            Assert.That(_normalPieces, Has.All.Matches((IPiece p) => p.IsMoveable),
                "All 'normal' pieces should have 'IsMoveable' set to true.");

            Assert.That(_normalPieces, Has.All.Matches((IPiece p) => p.IsAlive),
                "All pieces should be alive after construction.");

            Assert.That(_normalPieces, Has.All.Matches((IPiece p) => p.Id != Guid.Empty),
                "All pieces should have a unique 'Id' after construction.");

            Assert.That(_normalPieces.Select(p => p.Id), Is.Unique,
                "All pieces should have a unique 'Id' after construction.");

            Assert.That(_normalPieces.Select(p => p.Position), Has.All.Null,
                "All pieces should have a 'Position' equal to 'null' after construction.");
        }

        [MonitoredTest("Inheritance - All classes except Flag and Bomb should inherit from MoveablePiece class ")]
        public void MoveablePieceClasses_ShouldInheritFromMoveablePiece()
        {
            Assert.IsInstanceOf<MoveablePiece>(_captain, "The Captain class should inherit from the MoveablePiece class");
            Assert.IsInstanceOf<MoveablePiece>(_colonel, "The Colonel class should inherit from the MoveablePiece class");
            Assert.IsInstanceOf<MoveablePiece>(_general, "The General class should inherit from the MoveablePiece class");
            Assert.IsInstanceOf<MoveablePiece>(_lieutenant, "The Lieutenant class should inherit from the MoveablePiece class");
            Assert.IsInstanceOf<MoveablePiece>(_major, "The Major class should inherit from the MoveablePiece class");
            Assert.IsInstanceOf<MoveablePiece>(_marshal, "The Marshal class should inherit from the MoveablePiece class");
            Assert.IsInstanceOf<MoveablePiece>(_scout, "The Scout class should inherit from the MoveablePiece class");
            Assert.IsInstanceOf<MoveablePiece>(_colonel, "The Colonel class should inherit from the MoveablePiece class");
            Assert.IsInstanceOf<MoveablePiece>(_sergeant, "The Sergeant class should inherit from the MoveablePiece class");
            Assert.IsInstanceOf<MoveablePiece>(_spy, "The Spy class should inherit from the MoveablePiece class");
        }

        [MonitoredTest("Attack - Against piece with lower strength - Should kill the other piece")]
        public void Attack_AgainstPieceWithLowerStrength_ShouldKillTheOtherPiece()
        {
            IPiece piece = SelectRandomPiece(out int selectedIndex);
            IPiece opponent = _normalPieces[Random.Next(0, selectedIndex)];

            piece.Attack(opponent);

            Assert.That(piece.IsAlive, Is.True, $"'{piece.Name}' should be alive after attacking a '{opponent.Name}'.");
            Assert.That(opponent.IsAlive, Is.False, $"'{opponent.Name}' should not be alive after being attacked by '{piece.Name}'.");
        }

        [MonitoredTest("Attack - Against piece with same strength - Should kill both pieces")]
        public void Attack_AgainstPieceWithSameStrength_ShouldKillBothPieces()
        {
            IPiece piece = SelectRandomPiece(out int _);
            IPiece opponent = new PieceMockBuilder()
                .WithStrength(piece.Strength)
                .WithName(piece.Name)
                .Mock.Object;

            piece.Attack(opponent);

            Assert.That(piece.IsAlive, Is.False, $"'{piece.Name}' should be not be alive after attacking a '{opponent.Name}'.");
            Assert.That(opponent.IsAlive, Is.False, $"'{opponent.Name}' should not be alive after being attacked by '{piece.Name}'.");
        }

        [MonitoredTest("Attack - Against piece with higher strength - Should kill the attacking piece")]
        public void Attack_AgainstPieceWithHigherStrength_ShouldKillTheAttackingPiece()
        {
            IPiece piece = SelectRandomPiece(out int selectedIndex);
            IPiece opponent = _normalPieces[Random.Next(selectedIndex + 1, _normalPieces.Count)];

            piece.Attack(opponent);

            Assert.That(piece.IsAlive, Is.False, $"'{piece.Name}' should be not be after attacking a '{opponent.Name}'.");
            Assert.That(opponent.IsAlive, Is.True, $"'{opponent.Name}' should be alive after being attacked by '{piece.Name}'.");
        }

        private IPiece SelectRandomPiece(out int selectedIndex)
        {
            selectedIndex = Random.Next(1, _normalPieces.Count - 1);
            return _normalPieces[selectedIndex];
        }
    }
}