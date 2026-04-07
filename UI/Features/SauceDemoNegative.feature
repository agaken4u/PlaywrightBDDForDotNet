Feature: Sauce Demo Negative Scenarios
  As a user
  I want to see proper error handling and validations
  So that the application behaves correctly when something goes wrong

  Background:
    Given I am on the Sauce Demo login page

  # =======================
  # LOGIN NEGATIVE SCENARIOS
  # =======================
  Scenario: Login with invalid username
    When I enter username "invalid_user"
    And I enter password "secret_sauce"
    And I click the login button
    Then I should see the login error message "Epic sadface: Username and password do not match any user in this service"

  Scenario: Login with invalid password
    When I enter username "standard_user"
    And I enter password "wrong_password"
    And I click the login button
    Then I should see the login error message "Epic sadface: Username and password do not match any user in this service"

  Scenario: Login with empty username
    When I enter username ""
    And I enter password "secret_sauce"
    And I click the login button
    Then I should see the login error message "Epic sadface: Username is required"

  Scenario: Login with empty password
    When I enter username "standard_user"
    And I enter password ""
    And I click the login button
    Then I should see the login error message "Epic sadface: Password is required"

  # =======================
  # CART NEGATIVE SCENARIOS
  # =======================
  Scenario: Checkout with empty cart
    Given I am logged in as "standard_user" with password "secret_sauce"
    When I go to the cart page
    And I click the checkout button
    Then I should see the checkout page error message "Your cart is empty"

  # =======================
  # CHECKOUT NEGATIVE SCENARIOS
  # =======================
  Scenario: Checkout with missing first name
    Given I am logged in as "standard_user" with password "secret_sauce"
    And I add "sauce-labs-backpack" to the cart
    When I enter checkout info with "" "Doe" "12345"
    And I click the continue button
    Then I should see the checkout error message "Error: First Name is required"

  Scenario: Checkout with missing last name
    Given I am logged in as "standard_user" with password "secret_sauce"
    And I add "sauce-labs-backpack" to the cart
    When I enter checkout info with "John" "" "12345"
    And I click the continue button
    Then I should see the checkout error message "Error: Last Name is required"

  Scenario: Checkout with missing postal code
    Given I am logged in as "standard_user" with password "secret_sauce"
    And I add "sauce-labs-backpack" to the cart
    When I enter checkout info with "John" "Doe" ""
    And I click the continue button
    Then I should see the checkout error message "Error: Postal Code is required"