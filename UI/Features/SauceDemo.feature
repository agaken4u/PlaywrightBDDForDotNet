Feature: Sauce Demo Application
  As a user
  I want to interact with the Sauce Demo web application
  So that I can login, browse products, manage my cart, and complete checkout successfully

  Background:
    Given I am on the Sauce Demo login page

  # =======================
  # LOGIN SCENARIOS
  # =======================
  Scenario: Successful login with valid credentials
    Given I am on the Sauce Demo login page
    When I enter username "standard_user"
    And I enter password "secret_sauce"
    And I click the login button
    Then I should be redirected to the inventory page
    And I should see the product list on the inventory page

  Scenario: Successful login with problem user
    Given I am on the Sauce Demo login page
    When I enter username "problem_user"
    And I enter password "secret_sauce"
    And I click the login button
    Then I should be redirected to the inventory page
    And I should see the product list on the inventory page

  # =======================
  # PRODUCT INTERACTION SCENARIOS
  # =======================
  Scenario: Add a single product to the cart
    Given I am logged in as "standard_user" with password "secret_sauce"
    When I add "sauce-labs-backpack" to the cart
    Then the cart badge should display "1"

  Scenario: Add multiple products to the cart
    Given I am logged in as "standard_user" with password "secret_sauce"
    When I add "sauce-labs-backpack" to the cart
    And I add "sauce-labs-bike-light" to the cart
    Then the cart badge should display "2"

  Scenario: Remove a product from the cart
    Given I am logged in as "standard_user" with password "secret_sauce"
    And I add "sauce-labs-backpack" to the cart
    When I remove "sauce-labs-backpack" from the cart
    Then the cart badge should display "0"

  # =======================
  # CHECKOUT SCENARIOS
  # =======================
  Scenario: Complete checkout with valid information
    Given I am logged in as "standard_user" with password "secret_sauce"
    And I add "sauce-labs-backpack" to the cart
    When I complete checkout with "John", "Doe", "12345"
    Then I should see the order confirmation message "Thank you for your order!"

  Scenario: Checkout with multiple items
    Given I am on the Sauce Demo login page
    And I am logged in as "standard_user" with password "secret_sauce"
    When I add "sauce-labs-backpack" to the cart
    And I add "sauce-labs-bike-light" to the cart
    And I complete checkout with "Jane", "Smith", "54321"
    Then I should see the order confirmation message "Thank you for your order!"

  # =======================
  # LOGOUT SCENARIO
  # =======================
  Scenario: Logout from the application
    Given I am logged in as "standard_user" with password "secret_sauce"
    When I logout
    Then I should be redirected to the login page