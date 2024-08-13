# Onyx SDCT
Software developer coding test for Onyx Commodities

## Requirements
- Create a .NET 6+ “Products” Web API including the following:
  - Anonymous endpoint: Health check/OK endpoint
  - Secured endpoints (using your implementation of choice) to:
    - Create a product
    - Return a list of all products in JSON format
    - Include a way to retrieve all products of a specific colour only
- Appropriate unit and integration tests
- Include a simple architecture diagram showing how this products service could form part of a distributed or microservices event-driven architecture with a few other components shown (e.g. orders, payments)
- Push to a public repo on GitHub and send the link

## Application
### Builded using
- .NET 8
- Minimal APIs 
- xUnit
- FluentAssertion
- FluentValidation
- Swashbuckle (Swagger)

### Featues
- Swagger interface (https://localhost:7274/swagger) with autorization
- Simple in-memory storage scheme
- Basic CRUD endpoints
- Annonymous health check endpoint in "/health" -> HTTP 200 OK

- Uses in-header API Key authorization method for simplicity
  - Header: X-Api-Key
  - Key: d48b3f23c6094247a9eef315d856664e

- GET "/products" endpoint allows filter products by color (as requested) and/or name
  - "/product?name=Shirt" -> returns all products named "Shirt" (case insensitive)
  - "/product?color=red" -> returns all products with color "red" (case insensitive)
  - "/product?name=hat&color=black" -> returns all products named "hat" AND color "black" (case insensitive)

- Unit tests (repository) and integration tests (API)

- Architecture diagram in "Architecture.png"
