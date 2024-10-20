Features

1. Navigation Bar

Dashboard (or Home):
Displays items to trade, categorized into three sections:
Forex
Crypto
Stocks
These categories will show live market rates fetched from the backend API.
Accessible to everyone, even without logging in.
Note: Users can view rates but cannot place orders without logging in.

Portfolio:
Disabled until the user is logged in (i.e., a session is established and the userId is defined).
Displays user’s current portfolio, wallet balance, and open positions.
Shows uninvested funds in USD by default, updated based on investment activity.

Login/Logout:
If the user is not logged in, the button displays Login.
On clicking Login, users can either log in or register a new account.
After logging in, the button switches to Logout.
On clicking Logout, the session is terminated, and user-specific pages like the portfolio are no longer accessible.


2. Authentication & Session Management
Users can log in and register via the frontend.
Once authenticated, a JWT token is stored and used for accessing protected APIs.
The JWT token is cached in Redis on the backend, which will be accessed by the React app to verify the session status and make authenticated requests.
Logout clears the session and terminates access to user-specific functionality.


3. Real-Time Data
The app fetches market data (forex, crypto, stocks) from Redis, which caches this data for performance optimization.
The real-time rates are updated on the dashboard page.


Technologies
Frontend:
React: For building UI components.
React Router: For managing navigation (e.g., routing between Dashboard, Portfolio, Login pages).
Axios: For making API requests to the backend services.