using System;
using NUnit.Framework;
using Stratego.Domain.ArmyDomain;
using Stratego.Domain.ArmyDomain.Contracts;
using Stratego.TestTools.Builders;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy.Internal;
using Guts.Client.Shared;
using Stratego.TestTools;

namespace Stratego.Domain.Tests
{
    public class ArmyTests
    {
        private static readonly Random RandomGenerator = new Random();

        private Army _army;
        private FieldInfo _piecesField;

        [SetUp]
        public void Setup()
        {
            _army = new Army(true);
            _piecesField = typeof(Army).GetAllFields().FirstOrDefault(f => f.IsPrivate && f.FieldType == typeof(IPiece[]));
        }

        [MonitoredTest("Constructor - QuickGame - Should create army of 10 pieces")]
        public void Constructor_QuickGame_ShouldCreateArmyOf10Pieces()
        {
            IPiece[] pieces = GetArmyPieces(_army);

            Assert.That(pieces.Count, Is.EqualTo(10), "Quickgame must have 10 pieces alive");
            Assert.That(pieces.Where(p => p.Name == "Flag").Count, Is.EqualTo(1), "There should be 1 piece with name 'Flag' in the army");
            Assert.That(pieces.Where(p => p.Name == "Bomb").Count, Is.EqualTo(2), "There should be 2 pieces with name 'Bomb' in the army");
            Assert.That(pieces.Where(p => p.Name == "Spy").Count, Is.EqualTo(1), "There should be 1 piece with name 'Spy' in the army");
            Assert.That(pieces.Where(p => p.Name == "Scout").Count, Is.EqualTo(2), "There should be 2 pieces with name 'Scout' in the army");
            Assert.That(pieces.Where(p => p.Name == "Miner").Count, Is.EqualTo(2), "There should be 2 pieces with name 'Miner' in the army");
            Assert.That(pieces.Where(p => p.Name == "General").Count, Is.EqualTo(1), "There should be 1 piece with name 'General' in the army");
            Assert.That(pieces.Where(p => p.Name == "Marshal").Count, Is.EqualTo(1), "There should be 1 piece with name 'Marshal' in the army");
        }

        [MonitoredTest("FindPieces - Some pieces are death - Should only return pieces with matching IsAlive property")]
        public void FindPieces_SomePiecesAreDeath_ShouldOnlyReturnPiecesWithMatchingIsAliveProperty()
        {
            IPiece[] pieces =
            {
                new PieceMockBuilder().WithAlive(true).Mock.Object,
                new PieceMockBuilder().WithAlive(false).Mock.Object,
                new PieceMockBuilder().WithAlive(true).Mock.Object,
                new PieceMockBuilder().WithAlive(false).Mock.Object
            };
            SetArmyPieces(_army, pieces);

            //Act
            var deathPieces = _army.FindPieces(false);
            var alivePieces = _army.FindPieces(true);

            //Assert
            Assert.That(deathPieces.All(p => !p.IsAlive), Is.True, "When the alive parameter is false, only death pieces should be returned");
            Assert.That(deathPieces.Count, Is.EqualTo(2), "When the alive parameter is false, all death pieces should be returned");

            Assert.That(alivePieces.All(p => p.IsAlive), Is.True, "When the alive parameter is true, only alive pieces should be returned");
            Assert.That(alivePieces.Count, Is.EqualTo(2), "When the alive parameter is true, all alive pieces should be returned");
        }

        [MonitoredTest("GetPieceById - Should return the piece with the matching Id")]
        public void GetPieceById_ShouldReturnThePieceWithTheMatchingId()
        {
            //Arrange
            var pieces = BuildRandomPiecesArray();
            SetArmyPieces(_army, pieces);
            IPiece pieceToFind = pieces.NextRandomElement();

            //Act
            IPiece result = _army.GetPieceById(pieceToFind.Id);

            //Assert
            Assert.That(result, Is.Not.Null, "No piece was returned");
            Assert.That(result.Id, Is.EqualTo(pieceToFind.Id), "The returned piece has the wrong id");
        }

        [MonitoredTest("GetPieceById - Should throw ApplicationException when the piece is not found")]
        public void GetPieceById_ShouldThrowApplicationExceptionWhenThePieceIsNotFound()
        {
            //Arrange
            var pieces = BuildRandomPiecesArray();
            SetArmyPieces(_army, pieces);
            IPiece otherPiece = new PieceMockBuilder().Mock.Object;

            //Act + Assert
            Assert.That(() => _army.GetPieceById(otherPiece.Id),
                Throws.InstanceOf<ApplicationException>().With.Message.Not.Empty,
                "No ApplicationException is thrown or the exception has an empty message");
        }

        [MonitoredTest("IsPositionedOnBoard - All pieces on the board - Should return true")]
        public void IsPositionedOnBoard_AllPiecesOnTheBoard_ShouldReturnTrue()
        {
            //Arrange
            var pieces = BuildRandomPiecesArray(onBoard: true);
            SetArmyPieces(_army, pieces);

            //Act
            bool isPositionedOnBoard = _army.IsPositionedOnBoard;

            //Assert
            Assert.That(isPositionedOnBoard, Is.True);
        }

        [MonitoredTest("IsPositionedOnBoard - Not all pieces on the board - Should return false")]
        public void IsPositionedOnBoard_NotAllPiecesOnTheBoard_ShouldReturnFalse()
        {
            //Arrange
            IPiece[] positionedPieces = BuildRandomPiecesArray(onBoard: true);
            IPiece[] otherPieces = BuildRandomPiecesArray(onBoard: false);
            IPiece[] allPieces = otherPieces.Union(positionedPieces).ToArray();
            SetArmyPieces(_army, allPieces);

            //Act
            bool isPositionedOnBoard = _army.IsPositionedOnBoard;

            //Assert
            Assert.That(isPositionedOnBoard, Is.False);
        }

        [MonitoredTest("IsDefeated - Flag is dead - Should return true")]
        public void IsDefeated_FlagIsDead_ShouldReturnTrue()
        {
            //Arrange
            IPiece[] pieces = GetArmyPieces(_army);
            Flag flag = pieces.OfType<Flag>().FirstOrDefault();
            Assert.That(flag, Is.Not.Null, "Could not find a Flag in the army.");
            flag.IsAlive = false;

            //Act
            bool isDefeated = _army.IsDefeated;

            //Assert
            Assert.That(isDefeated, Is.True);
        }

        [MonitoredTest("IsDefeated - All moveable pieces are dead - Should return true")]
        public void IsDefeated_AllMoveablePiecesAreDead_ShouldReturnTrue()
        {
            //Arrange
            IPiece[] allPieces = GetArmyPieces(_army);

            Flag flag = allPieces.OfType<Flag>().FirstOrDefault();
            Assert.That(flag, Is.Not.Null, "Could not find a Flag in the army.");
            flag.IsAlive = true;

            var moveablePieces = allPieces.Where(p => p.IsMoveable).ToList();
            foreach (IPiece moveablePiece in moveablePieces)
            {
                moveablePiece.IsAlive = false;
            }
            
            //Act
            bool isDefeated = _army.IsDefeated;

            //Assert
            Assert.That(isDefeated, Is.True);
        }

        [MonitoredTest("IsDefeated - Flag and some moveable pieces are alive - Should return false")]
        public void IsDefeated_FlagAndSomeMoveablePiecesAreAlive_ShouldReturnFalse()
        {
            //Arrange
            IPiece[] allPieces = GetArmyPieces(_army);

            Flag flag = allPieces.OfType<Flag>().FirstOrDefault();
            Assert.That(flag, Is.Not.Null, "Could not find a Flag in the army.");
            flag.IsAlive = true;

            var moveablePieces = allPieces.Where(p => p.IsMoveable).ToList();
            moveablePieces.ForEach(p => p.IsAlive = false);
            foreach (IPiece moveablePiece in moveablePieces.Take(RandomGenerator.Next(1, moveablePieces.Count)))
            {
                moveablePiece.IsAlive = true;
            }

            //Act
            bool isDefeated = _army.IsDefeated;

            //Assert
            Assert.That(isDefeated, Is.False);
        }

        private void AssertUsesArrayOfIPieceField()
        {
            Assert.That(_piecesField, Is.Not.Null, "The class should have private field of type IPiece[] that contains the pieces of the army.");
        }

        private IPiece[] GetArmyPieces(Army army)
        {
            AssertUsesArrayOfIPieceField();
            return (IPiece[]) _piecesField.GetValue(army);
        }

        private void SetArmyPieces(Army army, IPiece[] pieces)
        {
            AssertUsesArrayOfIPieceField();
            _piecesField.SetValue(army, pieces);
        }

        private IPiece[] BuildRandomPiecesArray(bool? onBoard = null)
        {
            IPiece[] pieces = new IPiece[RandomGenerator.Next(5,11)];
            for (int i = 0; i < pieces.Length; i++)
            {
                var pieceMockBuilder = new PieceMockBuilder();
                if (onBoard.HasValue)
                {
                    var position = onBoard.Value ? new BoardCoordinateBuilder().Build() : null;
                    pieceMockBuilder.WithPosition(position);
                }
                pieces[i] = pieceMockBuilder.Object;
            }
            return pieces;
        }

    }
}
