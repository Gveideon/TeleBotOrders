from Dish import Dish
from Menu import Menu
from Cafe import Cafe
from bs4 import BeautifulSoup

def Parse(page_source):
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

    #Cafe('Китайская забегаловка', Menu('Китайская забегаловка' ,list_result))