from importlib.resources import contents
import os, time, json
from bs4 import BeautifulSoup
from selenium import webdriver
from selenium.webdriver.firefox.options import Options

list_url = ["https://dubna-china.ru", "https://dodopizza.ru/dubna"]

class Parser:

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

    def Parse_CE(self, page_source):
        soup = BeautifulSoup(page_source, "lxml")
        list_result = []
        list__dishes = soup.select('div.sc-bczRLJ.kxPUqq')
        for element in list__dishes:
            sibling = element.find_next_sibling()
            img = element.next.attrs['src']
            name = sibling.contents[0].contents[0]
            count = sibling.contents[0].contents[1].text
            if len(sibling.contents[1].contents) == 0:
                description = ''
            else:    
                description = sibling.contents[1].contents[0]
            price = sibling.contents[2].contents[0].contents[0].text
            list_result.append([name, count, description, img, price])
        return list_result

    def Parse_DODO(self, page_source):
        soup = BeautifulSoup(page_source, "lxml")
        list_result = []
        list__dishes = soup.select('section.sc-1n2d0ov-2.bxiXBh')
        for element in list__dishes:
            for item in element.contents:
                if item.attrs['class'][-1] == 'ODyiI':
                    img = ''
                    name = item.contents[2].contents[0].text
                    count = ''
                    description = item.contents[2].contents[1].text
                    price = item.contents[2].contents[2].contents[0]
                else:
                    try:
                        img = item.contents[0].contents[0].contents[0]['srcset'].split()[0]
                    except Exception:
                        print('no good')
                    finally:
                        img = ''
                        name = item.contents[0].contents[1].text
                        count = ''
                        description = item.contents[0].contents[2:]
                        price = item.contents[1].contents[0].contents[0]
                list_result.append([name, count, description, img, price])
        return list_result

    def get_cafes(self, url):
        page_source = self.__get_page_source(url, self.__get_browser())
        if url == "https://dubna-china.ru":
              result = self.Parse_CE(page_source)
        if url == "https://dodopizza.ru/dubna":
            result = self.Parse_DODO(page_source)
        return result

    

if __name__ == "__main__":
    parser = Parser()
    cafe_CE = parser.get_cafes(list_url[0])
    with open("data_file_CE.json", "w") as write_file:
         json.dump(cafe_CE, write_file)
    print("good")
    cafe_DODO = parser.get_cafes(list_url[1])
    with open("data_file_DODO.json", "w") as write_file:
        json.dump(cafe_DODO, write_file)
    print("good")

