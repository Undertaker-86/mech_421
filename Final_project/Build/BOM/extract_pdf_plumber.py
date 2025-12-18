
import pdfplumber

pdf_path = r'd:\Repository\mech_421\Final_project\Build\BOM\BOM List Transformer - Tabellenblatt1-2.pdf'

try:
    with pdfplumber.open(pdf_path) as pdf:
        for page in pdf.pages:
            table = page.extract_table()
            if table:
                with open('bom_content.txt', 'w', encoding='utf-8') as out_f:
                    for row in table:
                        out_f.write(str(row) + "\n")
            else:
                with open('bom_content.txt', 'w', encoding='utf-8') as out_f:
                    out_f.write(page.extract_text())
except Exception as e:
    print(f"Error: {e}")
