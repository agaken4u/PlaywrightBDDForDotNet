Feature: Petstore API - Comprehensive CRUD and Robustness Testing
  As a user of the Petstore API
  I want to validate all CRUD operations and edge cases
  So that the API is reliable, secure, and handles errors gracefully

  Background:
    Given the Petstore API is available

  ############################################
  # CREATE (POST /pet)
  ############################################

  @create @positive
  Scenario: Create a new pet with valid data
    When I send a POST request to "/pet" with payload:
      """
      {
        "id": 1001,
        "name": "Buddy",
        "photoUrls": ["https://example.com/photo.jpg"],
        "status": "available"
      }
      """
    Then the response status should be 200
    And the response should contain "id" as 1001
    And the response should contain "name" as "Buddy"
    And the response should contain "status" as "available"

  @create @negative
  Scenario: Create a pet with missing required fields
    When I send a POST request to "/pet" with payload:
      """
      {
        "id": 1002
      }
      """
    Then the response status should be 400
    And the response should contain an error message

  @create @negative
  Scenario: Create a pet with invalid data type
    When I send a POST request to "/pet" with payload:
      """
      {
        "id": "invalid",
        "name": "Buddy"
      }
      """
    Then the response status should be 400
    And the response should contain an error message

  @create @edge
  Scenario: Create pet with duplicate ID
    Given a pet exists with id 2001
    When I send a POST request to "/pet" with payload:
      """
      {
        "id": 2001,
        "name": "DuplicatePet"
      }
      """
    Then the response status should be one of [400,409]
    And the response should contain an error message

  @create @edge
  Scenario: Create pet with large payload
    When I send a POST request to "/pet" with payload:
      """
      {
        "id": 3001,
        "name": "VeryLargeName_XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
        "status": "available"
      }
      """
    Then the response status should be one of [200,400]

  @create @edge
  Scenario: Create pet with special characters in name
    When I send a POST request to "/pet" with payload:
      """
      {
        "id": 3002,
        "name": "@#$%^&*()!",
        "status": "available"
      }
      """
    Then the response status should be one of [200,400]

  ############################################
  # READ (GET /pet/{petId})
  ############################################

  @read @positive
  Scenario: Retrieve an existing pet
    Given a pet exists with id 1001
    When I send a GET request to "/pet/1001"
    Then the response status should be 200
    And the response should contain "id" as 1001

  @read @negative
  Scenario: Retrieve a non-existent pet
    When I send a GET request to "/pet/999999"
    Then the response status should be 404
    And the response should contain an error message

  @read @negative
  Scenario: Retrieve pet with invalid ID format
    When I send a GET request to "/pet/abc"
    Then the response status should be one of [400,404]
    And the response should contain an error message

  ############################################
  # UPDATE (PUT /pet)
  ############################################

  @update @positive
  Scenario: Update an existing pet
    Given a pet exists with id 1001
    When I send a PUT request to "/pet" with payload:
      """
      {
        "id": 1001,
        "name": "Max",
        "status": "sold"
      }
      """
    Then the response status should be 200
    And the response should contain "name" as "Max"
    And the response should contain "status" as "sold"

  @update @negative
  Scenario: Update a pet with invalid payload
    When I send a PUT request to "/pet" with payload:
      """
      {
        "id": 1001,
        "name": 12345
      }
      """
    Then the response status should be 400
    And the response should contain an error message

  @update @edge
  Scenario: Update a deleted pet
    Given a pet with id 4001 has been deleted
    When I send a PUT request to "/pet" with payload:
      """
      {
        "id": 4001,
        "name": "GhostPet"
      }
      """
    Then the response status should be one of [200,404,400]

  ############################################
  # DELETE (DELETE /pet/{petId})
  ############################################

  @delete @positive
  Scenario: Delete an existing pet
    Given a pet exists with id 1001
    When I send a DELETE request to "/pet/1001"
    Then the response status should be 200

  @delete @negative
  Scenario: Delete a non-existent pet
    When I send a DELETE request to "/pet/999999"
    Then the response status should be one of [200,404,400]

  ############################################
  # ERROR HANDLING & SECURITY
  ############################################

  @errorHandling
  Scenario: Send malformed JSON
    When I send a POST request to "/pet" with invalid JSON
    Then the response status should be one of [400,500]
    And the response should contain an error message

  @security
  Scenario: Access API without proper headers
    When I send a POST request to "/pet" without headers
    Then the response status should be one of [401,403,404,415]

  ############################################
  # PERFORMANCE
  ############################################

  @performance
  Scenario: Validate API response time
    When I send a GET request to "/pet/1"
    Then the response time should be less than 2000 milliseconds