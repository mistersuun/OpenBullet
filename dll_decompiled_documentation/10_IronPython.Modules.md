# IronPython.Modules.dll Documentation

## Overview
IronPython.Modules.dll provides the Python standard library implementation for IronPython, including essential modules like os, sys, re, datetime, hashlib, and many others that Python developers expect to have available.

## Purpose in OpenBullet
- Provide Python standard library functionality
- Enable complex data processing in Python scripts
- Support file operations, networking, and cryptography
- Allow regex operations and date/time handling
- Enable Python-based data transformations

## Key Modules

### Core System Modules

#### `os` Module
- **Purpose**: Operating system interface
- **Key Functions**:
```python
import os

# Environment variables
api_key = os.environ.get('API_KEY')
os.environ['CUSTOM_VAR'] = 'value'

# Path operations
current_dir = os.getcwd()
os.path.join('folder', 'file.txt')
os.path.exists('file.txt')
os.path.isfile('data.json')
os.path.dirname('/path/to/file')

# File operations
os.remove('temp.txt')
os.rename('old.txt', 'new.txt')
os.makedirs('path/to/directory')
```

#### `sys` Module
- **Purpose**: System-specific parameters
- **Key Features**:
```python
import sys

# Python version info
version = sys.version
platform = sys.platform

# Path management
sys.path.append('/custom/module/path')

# Command line arguments
args = sys.argv

# Exit program
sys.exit(0)
```

### Data Processing Modules

#### `re` Module (Regular Expressions)
- **Purpose**: Pattern matching and text processing
- **Implementation**:
```python
import re

def extract_data(text):
    # Find all emails
    emails = re.findall(r'[\w\.-]+@[\w\.-]+\.\w+', text)
    
    # Extract tokens
    token_pattern = r'token["\']?\s*[:=]\s*["\']?([a-zA-Z0-9]+)'
    tokens = re.findall(token_pattern, text, re.IGNORECASE)
    
    # Replace sensitive data
    masked = re.sub(r'\b\d{4}-\d{4}-\d{4}-\d{4}\b', 'XXXX-XXXX-XXXX-XXXX', text)
    
    # Validate format
    if re.match(r'^\d{3}-\d{2}-\d{4}$', ssn):
        return True
    
    return {
        'emails': emails,
        'tokens': tokens,
        'masked': masked
    }
```

#### `datetime` Module
- **Purpose**: Date and time handling
- **Usage**:
```python
from datetime import datetime, timedelta

# Current time
now = datetime.now()
utc_now = datetime.utcnow()

# Formatting
formatted = now.strftime('%Y-%m-%d %H:%M:%S')

# Parsing
parsed = datetime.strptime('2024-01-15', '%Y-%m-%d')

# Calculations
tomorrow = now + timedelta(days=1)
age = now - birthdate

# Timestamp conversion
timestamp = datetime.fromtimestamp(1234567890)
unix_time = now.timestamp()
```

### Cryptography and Encoding

#### `hashlib` Module
- **Purpose**: Cryptographic hashing
- **Implementation**:
```python
import hashlib
import hmac

def generate_hashes(data):
    # Various hash algorithms
    md5 = hashlib.md5(data.encode()).hexdigest()
    sha1 = hashlib.sha1(data.encode()).hexdigest()
    sha256 = hashlib.sha256(data.encode()).hexdigest()
    sha512 = hashlib.sha512(data.encode()).hexdigest()
    
    return {
        'md5': md5,
        'sha1': sha1,
        'sha256': sha256,
        'sha512': sha512
    }

def generate_hmac(data, secret):
    # HMAC for authentication
    signature = hmac.new(
        secret.encode(),
        data.encode(),
        hashlib.sha256
    ).hexdigest()
    return signature
```

#### `base64` Module
- **Purpose**: Base64 encoding/decoding
- **Usage**:
```python
import base64

# Encode
encoded = base64.b64encode(b'data to encode')
url_safe = base64.urlsafe_b64encode(b'url data')

# Decode
decoded = base64.b64decode(encoded)

# Custom encoding
def encode_credentials(username, password):
    credentials = f"{username}:{password}"
    return base64.b64encode(credentials.encode()).decode()
```

### Data Formats

#### `json` Module
- **Purpose**: JSON parsing and generation
- **Implementation**:
```python
import json

# Parse JSON
data = json.loads(json_string)

# Generate JSON
json_output = json.dumps(data, indent=2)

# Work with files
with open('data.json', 'r') as f:
    config = json.load(f)

# Custom encoding
class CustomEncoder(json.JSONEncoder):
    def default(self, obj):
        if isinstance(obj, datetime):
            return obj.isoformat()
        return super().default(obj)
```

#### `csv` Module
- **Purpose**: CSV file handling
- **Usage**:
```python
import csv

# Read CSV
def read_combos(filepath):
    combos = []
    with open(filepath, 'r') as f:
        reader = csv.reader(f, delimiter=':')
        for row in reader:
            if len(row) >= 2:
                combos.append({'user': row[0], 'pass': row[1]})
    return combos

# Write CSV
def write_results(results, filepath):
    with open(filepath, 'w', newline='') as f:
        writer = csv.DictWriter(f, fieldnames=['user', 'status', 'data'])
        writer.writeheader()
        writer.writerows(results)
```

### Network and Internet

#### `urllib` Module
- **Purpose**: URL handling and HTTP requests
- **Implementation**:
```python
import urllib.parse
import urllib.request

# URL encoding
params = urllib.parse.urlencode({'key': 'value', 'token': 'abc123'})
quoted = urllib.parse.quote('special characters!')

# Parse URLs
parsed = urllib.parse.urlparse('https://example.com/path?query=1')
domain = parsed.netloc
path = parsed.path

# Build URLs
base_url = 'https://api.example.com'
endpoint = '/users'
query = urllib.parse.urlencode({'id': 123})
full_url = f"{base_url}{endpoint}?{query}"
```

### Mathematical Operations

#### `math` Module
- **Purpose**: Mathematical functions
- **Usage**:
```python
import math

# Basic operations
result = math.sqrt(16)
power = math.pow(2, 10)
logarithm = math.log(100, 10)

# Trigonometry
sine = math.sin(math.pi / 2)
cosine = math.cos(0)

# Rounding
ceil = math.ceil(4.3)  # 5
floor = math.floor(4.7)  # 4

# Statistics
import statistics
mean = statistics.mean([1, 2, 3, 4, 5])
median = statistics.median([1, 2, 3, 4, 5])
stdev = statistics.stdev([1, 2, 3, 4, 5])
```

### Random Generation

#### `random` Module
- **Purpose**: Random number generation
- **Implementation**:
```python
import random
import string

# Random numbers
random_int = random.randint(1, 100)
random_float = random.random()

# Random choice
item = random.choice(['option1', 'option2', 'option3'])

# Shuffle
items = [1, 2, 3, 4, 5]
random.shuffle(items)

# Generate random string
def generate_random_string(length=10):
    chars = string.ascii_letters + string.digits
    return ''.join(random.choice(chars) for _ in range(length))

# Generate random user agent
user_agents = [
    'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36',
    'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36'
]
random_ua = random.choice(user_agents)
```

## Integration with OpenBullet

### Python Script Block Usage
```python
# In OpenBullet SCRIPT block
import hashlib
import json
import re
from datetime import datetime

# Access bot variables
username = variables['USER']
password = variables['PASS']
response = variables['RESPONSE']

# Process response
try:
    data = json.loads(response)
    
    # Extract token
    token = data.get('access_token')
    if token:
        variables['TOKEN'] = token
        
        # Generate signature
        signature = hashlib.sha256(f"{username}:{token}".encode()).hexdigest()
        variables['SIGNATURE'] = signature
        
        # Set expiry
        expires = datetime.now() + timedelta(hours=1)
        variables['TOKEN_EXPIRES'] = expires.isoformat()
        
        status = 'SUCCESS'
    else:
        status = 'FAIL'
        
except json.JSONDecodeError:
    # Try regex extraction
    token_match = re.search(r'token["\']:\s*["\']([^"\']+)', response)
    if token_match:
        variables['TOKEN'] = token_match.group(1)
        status = 'SUCCESS'
    else:
        status = 'FAIL'

variables['STATUS'] = status
```

### Data Validation Functions
```python
import re

def validate_email(email):
    pattern = r'^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$'
    return bool(re.match(pattern, email))

def validate_phone(phone):
    # Remove non-digits
    digits = re.sub(r'\D', '', phone)
    return len(digits) == 10 or len(digits) == 11

def validate_credit_card(number):
    # Luhn algorithm
    digits = [int(d) for d in str(number)]
    checksum = 0
    
    for i, digit in enumerate(reversed(digits[:-1])):
        if i % 2 == 0:
            digit *= 2
            if digit > 9:
                digit -= 9
        checksum += digit
    
    return (checksum + digits[-1]) % 10 == 0
```

### Response Processing
```python
from bs4 import BeautifulSoup  # If available
import re
import json

def process_html_response(html):
    # Extract with regex if BeautifulSoup not available
    csrf_token = re.search(r'csrf_token["\']:\s*["\']([^"\']+)', html)
    session_id = re.search(r'sessionId["\']:\s*["\']([^"\']+)', html)
    
    # Extract form data
    form_action = re.search(r'<form[^>]+action=["\']([^"\']+)', html)
    input_fields = re.findall(r'<input[^>]+name=["\']([^"\']+)[^>]+value=["\']([^"\']+)', html)
    
    return {
        'csrf_token': csrf_token.group(1) if csrf_token else None,
        'session_id': session_id.group(1) if session_id else None,
        'form_action': form_action.group(1) if form_action else None,
        'form_data': dict(input_fields)
    }
```

## Advanced Features

### Custom Module Import
```python
# Import .NET assemblies
import clr
clr.AddReference("System.Web")
from System.Web import HttpUtility

# URL encode/decode
encoded = HttpUtility.UrlEncode("special chars & symbols")
decoded = HttpUtility.UrlDecode(encoded)
```

### Performance Optimization
```python
# Compile regex patterns
import re

# Compile once, use many times
EMAIL_PATTERN = re.compile(r'[\w\.-]+@[\w\.-]+\.\w+')
TOKEN_PATTERN = re.compile(r'token=([a-zA-Z0-9]+)')

def extract_fast(text):
    emails = EMAIL_PATTERN.findall(text)
    tokens = TOKEN_PATTERN.findall(text)
    return emails, tokens
```

## Best Practices
1. Import only needed modules
2. Compile regex patterns for reuse
3. Handle exceptions properly
4. Use appropriate data structures
5. Cache computed values
6. Validate input data
7. Use built-in functions when available

## Limitations
- Some C-extension modules not available
- Performance overhead vs CPython
- Python 2.7 syntax only
- Limited third-party library support

## Common Use Cases

### API Authentication
```python
import hashlib
import hmac
import time

def generate_api_signature(method, url, body, secret):
    timestamp = str(int(time.time()))
    message = f"{method}\n{url}\n{body}\n{timestamp}"
    
    signature = hmac.new(
        secret.encode(),
        message.encode(),
        hashlib.sha256
    ).hexdigest()
    
    return {
        'signature': signature,
        'timestamp': timestamp
    }
```

### Data Extraction
```python
import re
import json

def extract_json_from_html(html):
    # Find JSON in script tags
    json_pattern = r'<script[^>]*>.*?({["\'].*?})</script>'
    matches = re.findall(json_pattern, html, re.DOTALL)
    
    results = []
    for match in matches:
        try:
            data = json.loads(match)
            results.append(data)
        except:
            continue
    
    return results
```