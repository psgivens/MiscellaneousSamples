Feature: CalculatorSpecification
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

@Input
Scenario: Display two digits
	Given I have pressed the key '1'
	And I have pressed the key '0'
	When I look at the display
	Then I see the string "10"

@Input
Scenario: Do not display preceding zeros
	Given I have pressed the key '0'
	And I have pressed the key '1'
	When I look at the display
	Then I see the string "1" 

@Calculation
Scenario: Add 2 and 2
	Given I have pressed the key '2'
	And I have pressed the key '+'
	And I have pressed the key '2'
	And I have pressed the key '='
	When I look at the display
	Then I see the string "4"

@Calculation
Scenario: Multiply 2 and 2
	Given I have pressed the key '2'
	And I have pressed the key '*'
	And I have pressed the key '2'
	And I have pressed the key '='
	When I look at the display
	Then I see the string "4"

@Calculation
Scenario: Divide 4 by 2
	Given I have pressed the key '4'
	And I have pressed the key '/'
	And I have pressed the key '2'
	And I have pressed the key '='
	When I look at the display
	Then I see the string "2"


@Calculation
Scenario: Subtract 1 from 4
	Given I have pressed the key '4'
	And I have pressed the key '-'
	And I have pressed the key '1'
	And I have pressed the key '='
	When I look at the display
	Then I see the string "3"

@Anomolies
Scenario: Equals clears
	Given I have pressed the key '4'
	And I have pressed the key '='
	And I have pressed the key '5'
	And I have pressed the key '*'
	And I have pressed the key '6'
	And I have pressed the key '='
	When I look at the display
	Then I see the string "30"



