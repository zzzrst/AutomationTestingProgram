# ConcreteSteps
Please Refer to the [Api Documentation](/api/AutomationTestingProgram.AutomationFramework.html)  
* To Add More concrete Steps, simpily create a new class in "src/AutomationFramework/ConcreteTestSteps" that implements TestStep. 
* Override the name and name it your testStep name that it will be refered by.
* Also Override the Execute method but be sure to call `base.Execute()` or when the step is called, it will be stuck in an infinite loop.