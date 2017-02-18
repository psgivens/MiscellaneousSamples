Feature: IndonesianLanguageFeature
	I want to be sure that various corner cases 
	of indonesian numbers are tested.

@Specifications
Scenario: Translate the number 1
	When I translate 1 into bahasa Indonesia
	Then the result should be 'satu'
		
@Specifications
Scenario: Translate the number 345
	When I translate 345 into bahasa Indonesia
	Then the result should be 'tiga ratus empat puluh lima'

@Specifications
Scenario: Translate the number 115
	When I translate 115 into bahasa Indonesia
	Then the result should be 'seratus lima belas'

@Specifications
Scenario: Translate the number 111
	When I translate 111 into bahasa Indonesia
	Then the result should be 'seratus sebelas'

@Specifications
Scenario: Translate the number 75
	When I translate 75 into bahasa Indonesia
	Then the result should be 'tujuh puluh lima'
