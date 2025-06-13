To Test your Exception Handling in Mediatr Pipeline, you can use the following code:
    Create a handler that accepts a TestExceptionsRequest. 
    Then create an ExceptionTestHelper with a "static TestParamaters Params" property that contains the following:
        1. The container with your specific Handler and Exception. And a "Challenge" lamda that will test the reponse from the ExceptionHandler
    If you need to test whether logging occured, use  CustomLoggerMonitor.GetExceptionCount<MyHandler, MyException>()

    Add your ExceptionTestHelper to IdRequestExceptionHandlerTests.ExceptionHandlers
