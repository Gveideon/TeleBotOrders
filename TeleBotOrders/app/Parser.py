import os, time, Parser_for_CE
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
        browser.get(url)
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


    # page_source = __get_page_source(list_url[0], __get_browser())
    #     page_markup = BeautifulSoup(page_source, 'lxml')
    #     block_list = []
    #     for block in page_markup.find_all("div", "sc-bczRLJ hJYFVB"):
    #         name_block = block.find("div", "sc-bczRLJ dqChRk")
    #         block_list.append(name_block)
    #         for item in block.find_all("div", "sc-bczRLJ sc-gsnTZi Card-sc-2npckq-2 cisoKP jnFvAE jbjTQa"):
    #             item_list = []
    #             raw_name_dish = item.find("div","sc-bczRLJ gxeRtG")
    #             count = item.find("div", "sc-bczRLJ ANjLk")
    #             cost = item.find("div","sc-bczRLJ hWxACP")
    #             access = item.find("button","sc-bczRLJ sc-gsnTZi ButtonBase-sc-18dwnt0-0 cnfqzz jnFvAE iplRfl")
    #             img_source = item.find("img","sc-bczRLJ jBqnLQ Image-sc-2npckq-4 kPlshG")
    #             if raw_name_dish == None or count == None or cost == None or access == None or img_source == None:
    #                 continue
    #             name_dish = raw_name_dish.replace(count, "")
    #             item_list.extend([name_dish.text, count.text, cost.text, access.text, img_source["src"]])
    #             block_list.append(item_list)
    #     print(block_list)