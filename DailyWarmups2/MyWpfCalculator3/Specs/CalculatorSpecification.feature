Feature: CalculatorSpecification3
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

@Input
Scenario: Display two digits
	Given I have entered the keys "1" 
	When I enter the key "0"
	Then the display value will be "10" 

@Input
Scenario: Do not display preceding zeros
	Given I have entered the keys "0"
	When I enter the key "1"
	Then the display value will be "1"

@Calculation
Scenario: Add 2 and 2
	Given I have entered the keys "2+2"
	When I enter the key "="
	Then the display value will be "4"

@Calculation
Scenario: Multiply 2 and 2
	Given I have entered the keys "2*2"
	When I enter the key "="
	Then the display value will be "4"

@Calculation
Scenario: Divide 4 by 2
	Given I have entered the keys "4/2"
	When I enter the key "="
	Then the display value will be "2"
	
@Calculation
Scenario: Subtract 1 from 4
	Given I have entered the keys "4-1"
	When I enter the key "="
	Then the display value will be "3"
	
@OrderOfOperations
Scenario: Multiply and then add
	Given I have entered the keys "4*3+5"
	When I enter the key "="
	Then the display value will be "17"
		
@OrderOfOperations
Scenario: Add and then multiply
	Given I have entered the keys "5+4*3"
	When I enter the key "="
	Then the display value will be "17"

@OrderOfOperations
Scenario: Divide and then subtract
	Given I have entered the keys "12/3+5"
	When I enter the key "="
	Then the display value will be "9"

@OrderOfOperations
Scenario: Subtract and then divide
	Given I have entered the keys "5+4/2"
	When I enter the key "="
	Then the display value will be "7"

@OrderOfOperations
Scenario: Subtract and then add
	Given I have entered the keys "5-4+2"
	When I enter the key "="
	Then the display value will be "3"

@OrderOfOperations
Scenario: Add and then subtract
	Given I have entered the keys "5+4-2"
	When I enter the key "="
	Then the display value will be "7"
	

@OrderOfOperations
Scenario: Multiply, divide, add and subtract
	Given I have entered the keys "3*5+4/2-2*7"
	When I enter the key "="
	Then the display value will be "3"
	

@Anomolies
Scenario: Equals clears cache
	Given I have entered the keys "4=5*6"
	When I enter the key "="
	Then the display value will be "30"
	
@Anomolies
Scenario: Equals perserves display
	Given I have entered the keys "4"
	When I enter the key "="
	Then the display value will be "4"

