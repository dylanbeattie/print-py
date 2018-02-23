import os
from flask import Flask
from flask import request
from flask import render_template

import shutil

import requests

url = 'http://example.com/img.png'

app = Flask(__name__)

@app.route('/', methods=['GET','POST'])
def index():
    if request.method == 'POST':
        name = request.form['name']
        url = "http://192.168.10.181/home/label/" + name
        response = requests.get(url, stream=True)
        with open('output.png', 'wb') as out_file:
            shutil.copyfileobj(response.raw, out_file)
        del response
        # os.system("convert -background white -fill black -size 720x300 -pointsize 128 -gravity center label:\"{0}\" output.png".format(name))
        os.system("~/.local/bin/brother_ql_create --model QL-720NW ./output.png > output.bin")
        os.system("cat output.bin > /dev/usb/lp0")
        return 'Printed!'
    else:
        return render_template('form.html')

if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0')

