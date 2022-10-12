import os, time, json, Parser_for_CE
from selenium import webdriver
from selenium.webdriver.firefox.options import Options

list_url = ["https://dubna-china.ru", "https://dodopizza.ru/dubna"]

class Parser:
    def __init__(self, list_url_address):
        self.list_url = list_url_address

    def __get_browser(self):
        script_dir = os.path.dirname(__file__)
        rel_path = "drivers\geckodriver.exe"
        driver_path_exe = os.path.join(script_dir, rel_path)
        options = Options()
        options.add_argument("--headless")
        return webdriver.Firefox(options=options, executable_path = driver_path_exe)

    def __get_page_source(self, url, browser):
        try:
            browser.get(url)
        except Exception:
            print('ExeptionAccessForWeb')
        time.sleep(10)
        page_source = browser.page_source
        browser.close()
        browser.quit()
        return page_source

    def get_cafes(self):
        page_source = self.__get_page_source(list_url[0], self.__get_browser())
        return Parser_for_CE.Parse(page_source)


if __name__ == "__main__":
    parser = Parser(list_url)
    cafe = parser.get_cafes()
    with open("data_file.json", "w") as write_file:
        json.dump(cafe, write_file)
    print("good")
