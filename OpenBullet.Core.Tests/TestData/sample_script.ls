## Sample LoliScript for testing
## This script demonstrates various LoliScript features

#LOGIN REQUEST POST "https://example.com/login" 
  CONTENT "username=<USER>&password=<PASS>&remember=true" 
  CONTENTTYPE "application/x-www-form-urlencoded" 
  HEADER "User-Agent: TestBot/1.0"
  HEADER "Accept: text/html,application/xhtml+xml"

PARSE "<SOURCE>" LR "<title>" "</title>" -> VAR "PAGE_TITLE"

FUNCTION Hash SHA256 "<PASS>" -> VAR "PASSWORD_HASH"

#CHECK_RESULT KEYCHECK BanOn4XX=True BanOnToCheck=False
  KEYCHAIN Success OR 
    KEY "Welcome"
    KEY "Dashboard"
    KEY "Logged in successfully"
  KEYCHAIN Failure OR 
    KEY "Invalid credentials"
    KEY "Login failed"
    KEY "Incorrect username"
  KEYCHAIN Custom "2FA" OR 
    KEY "Two-factor authentication"
    KEY "Enter verification code"

PARSE "<SOURCE>" CSS "input[name='csrf_token']" "value" Recursive=True -> VAR "CSRF_TOKEN"

REQUEST GET "https://example.com/profile" 
  HEADER "Authorization: Bearer <TOKEN>"
  HEADER "X-CSRF-Token: <CSRF_TOKEN>"

PRINT Login completed with result: <RESULT>
PRINT User hash: <PASSWORD_HASH>
PRINT Page title: <PAGE_TITLE>
