Feature: Services Navigation
  As a user visiting the EPAM website
  I want to navigate to specific service categories
  So that I can validate the page content and expertise sections

@regression
Scenario Outline: Validate Navigation to Services Section
    Given I open the EPAM home page
    When I navigate to the Services page
    And I select the "<Category>" service category
    Then the page title should contain "<Category>"
    And the 'Our Related Offerings' section should be displayed

    Examples:
      | Category                |
      | Artificial Intelligence |