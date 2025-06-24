import time
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.chrome.service import Service
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from webdriver_manager.chrome import ChromeDriverManager

# Function to type character by character
def slow_type(element, text, delay=0.15):
    element.clear()
    for char in text:
        element.send_keys(char)
        time.sleep(delay)

# Configuration
url = "http://localhost:5103"
email = "testuser@example.com"
password = "Test@1234"

# Chrome setup
options = Options()
options.add_experimental_option("detach", True)  # Keep browser open
service = Service(ChromeDriverManager().install())
driver = webdriver.Chrome(service=service, options=options)

try:
    # 1. Go to Register Page
    driver.get(f"{url}/Identity/Account/Register")
    WebDriverWait(driver, 10).until(EC.presence_of_element_located((By.ID, "registerForm")))

    slow_type(driver.find_element(By.ID, "Input_Email"), email)
    slow_type(driver.find_element(By.ID, "Input_Password"), password)
    slow_type(driver.find_element(By.ID, "Input_ConfirmPassword"), password)

    time.sleep(1)
    driver.find_element(By.ID, "registerSubmit").click()

    # 2. Always go to Login after register
    time.sleep(2)
    driver.get(f"{url}/Identity/Account/Login")
    WebDriverWait(driver, 10).until(EC.presence_of_element_located((By.ID, "login-submit")))

    slow_type(driver.find_element(By.ID, "Input_Email"), email)
    slow_type(driver.find_element(By.ID, "Input_Password"), password)
    time.sleep(3)
    driver.find_element(By.ID, "login-submit").click()

    # 3. Go to Create Department
    # Wait for Departments index page
    WebDriverWait(driver, 10).until(EC.presence_of_element_located((By.LINK_TEXT, "Create New")))

    # Click the "Create New" button
    driver.find_element(By.LINK_TEXT, "Create New").click()
    time.sleep(1)
    # Now wait for the Create form to load
    WebDriverWait(driver, 10).until(EC.presence_of_element_located((By.ID, "Name")))


    slow_type(driver.find_element(By.ID, "Name"), "Automated Dept")
    slow_type(driver.find_element(By.ID, "Description"), "Created by automation")

    time.sleep(1)
    driver.find_element(By.XPATH, "//button[text()='Create']").click()

    print("✅ Registered, logged in, and department added successfully.")

except Exception as e:
    print(f"❌ Error occurred: {e}")
