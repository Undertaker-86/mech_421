
import PyPDF2
import re

pdf_path = r'd:\Repository\mech_421\Final_project\Build\BOM\BOM List Transformer - Tabellenblatt1-2.pdf'

try:
    with open(pdf_path, 'rb') as f:
        reader = PyPDF2.PdfReader(f)
        print(f"Number of pages: {len(reader.pages)}")
        for i, page in enumerate(reader.pages):
            print(f"--- Page {i+1} ---")
            text = page.extract_text()
            print(text)
            print("----------------")
except Exception as e:
    print(f"Error reading PDF: {e}")
