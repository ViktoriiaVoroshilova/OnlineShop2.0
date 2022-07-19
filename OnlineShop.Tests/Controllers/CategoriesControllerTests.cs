using AutoFixture;
using DataAccess.EF;
using DataAccess.EF.Models;
using DataAccess.EF.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineShop.Controllers;

namespace OnlineShop.Tests.Controllers
{
    [TestClass]
    public class CategoriesControllerTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
            _categoriesRepository = new Mock<IGenericRepository<Category>>(MockBehavior.Strict);
            _uow = new Mock<IUnitOfWork>(MockBehavior.Strict);
            _target = new CategoriesController(_uow.Object);
        }

        [TestMethod]
        public void GetCategoriesTest()
        {
            //arrange
            var categories = _fixture.CreateMany<Category>().ToList();

            _uow.Setup(x => x.CategoryRepository).Returns(_categoriesRepository.Object);
            _categoriesRepository.Setup(x => x.Get(null, null, string.Empty)).Returns(categories);

            //act
            var actual = _target.GetCategories();

            //assert
            actual.Value.Should().BeEquivalentTo(categories,
                $"Actual {string.Join(", ", actual.Value!)} is not equal expected {string.Join(", ", categories)}.");
            _uow.Verify(x => x.CategoryRepository, Times.Once);
            _categoriesRepository.Verify(x => x.Get(null, null, string.Empty), Times.Once);
        }

        [TestMethod]
        public void PutCategoryTest()
        {
            //arrange
            var category = _fixture.Create<Category>();

            _uow.Setup(x => x.CategoryRepository).Returns(_categoriesRepository.Object);
            _uow.Setup(x => x.Save());
            _categoriesRepository.Setup(x => x.Update(category));

            //act
            var actual = _target.PutCategory(category.Id, category);

            //assert
            (actual as ContentResult)?.StatusCode.Should().Be(204, "Status code is not NoContent");
            _uow.Verify(x => x.CategoryRepository, Times.Once);
            _uow.Verify(x => x.Save(), Times.Once);
            _categoriesRepository.Verify(x => x.Update(category), Times.Once);
        }

        [TestMethod]
        public void PutCategoryNullCategoryTest()
        {
            //arrange
            var category = null as Category;

            //act
            var actual = _target.PutCategory(_fixture.Create<int>(), category);

            //assert
            (actual as StatusCodeResult)?.StatusCode.Should().Be(400, "Status code is not NoContent");
        }

        [TestMethod]
        public void PutCategoryWrongCategoryIdTest()
        {
            //arrange
            var category = _fixture.Create<Category>();

            //act
            var actual = _target.PutCategory(_fixture.Create<int>(), category);

            //assert
            (actual as StatusCodeResult)?.StatusCode.Should().Be(400, "Status code is not NoContent");
        }

        [TestMethod]
        public void PutCategoryOptimisticExceptionTest()
        {
            //arrange
            var category = _fixture.Create<Category>();
            var exception = new DbUpdateConcurrencyException();

            _uow.Setup(x => x.CategoryRepository).Returns(_categoriesRepository.Object);
            _categoriesRepository.Setup(x => x.Update(category));
            _categoriesRepository.Setup(x => x.Find(category.Id)).Returns(null as Category);
            _uow.Setup(x => x.Save()).Throws(exception);

            //act
            var actual = _target.PutCategory(category.Id, category);

            //assert
            (actual as StatusCodeResult)?.StatusCode.Should().Be(404, "Status code is not NoContent");
            _uow.Verify(x => x.CategoryRepository, Times.Exactly(2));
            _uow.Verify(x => x.Save(), Times.Once);
            _categoriesRepository.Verify(x => x.Update(category), Times.Once);
            _categoriesRepository.Verify(x => x.Find(category.Id), Times.Once);
        }

        [TestMethod]
        public void PutCategoryOptimisticExceptionThrowTest()
        {
            //arrange
            var category = _fixture.Create<Category>();
            var exception = new DbUpdateConcurrencyException();

            _uow.Setup(x => x.CategoryRepository).Returns(_categoriesRepository.Object);
            _categoriesRepository.Setup(x => x.Update(category));
            _categoriesRepository.Setup(x => x.Find(category.Id)).Returns(category);
            _uow.Setup(x => x.Save()).Throws(exception);

            //act & assert
            _target.Invoking(x => x.PutCategory(category.Id, category))
                .Should()
                .Throw<DbUpdateConcurrencyException>();

            _uow.Verify(x => x.CategoryRepository, Times.Exactly(2));
            _uow.Verify(x => x.Save(), Times.Once);
            _categoriesRepository.Verify(x => x.Update(category), Times.Once);
            _categoriesRepository.Verify(x => x.Find(category.Id), Times.Once);
        }

        private Fixture _fixture = null!;
        private CategoriesController _target = null!;
        private Mock<IUnitOfWork> _uow = null!;
        private Mock<IGenericRepository<Category>> _categoriesRepository = null!;
    }
}