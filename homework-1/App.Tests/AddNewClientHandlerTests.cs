using AutoFixture.Xunit2;
using Moq;
using Ozon.ConsoleApp.Entities;
using Ozon.ConsoleApp.Handlers;
using Ozon.ConsoleApp.Services;

namespace App.Tests;

public class AddNewClientHandlerTests
{
    [Theory]
    [AutoData]
    public void Handle_WhenPassName_ShouldSaveClient(IAddNewClientHandler.Request request)
    {
        // Arrange
        var mock = new Mock<IClientStorage>();
        var cut = new AddNewClientHandler(mock.Object);
        
        //Act
        cut.Handle(request);

        //Assert
        mock.Verify(x => x.Save(It.Is<Client>(x => x.Name == request.Name)));
        mock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public void Handle_WhenNoPassName_ShouldThrow()
    {
        // Arrange
        var mock = new Mock<IClientStorage>();
        var cut = new AddNewClientHandler(mock.Object);
        
        //Act
        Assert.Throws<ArgumentException>(() => cut.Handle(new IAddNewClientHandler.Request() { Name = string.Empty }));
    }
}