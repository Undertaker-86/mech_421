import fitz
import sys
text = []
with fitz.open('slau395.pdf') as doc:
    for page in doc:
        text.append(page.get_text())
open('_tmp_slau.txt','w',encoding='utf-8').write('\n'.join(text))
