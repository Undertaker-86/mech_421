
import PyPDF2

pdf_path = r'd:\Repository\mech_421\Final_project\Build\BOM\BOM List Transformer - Tabellenblatt1-2.pdf'

try:
    with open(pdf_path, 'rb') as f:
        reader = PyPDF2.PdfReader(f)
        text = ""
        for page in reader.pages:
            text += page.extract_text() + "\n"
        print(text)
except Exception as e:
    print(f"Error reading PDF: {e}")
