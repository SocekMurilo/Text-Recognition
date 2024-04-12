from joblib import dump, load

from flask import (
    Blueprint, flash, g, redirect, render_template, request, session, url_for,
    jsonify
)


bp = Blueprint('json', __name__, url_prefix='/json')

@bp.route('/', methods=['POST'])
def requisition():
    data = request.get_json()

    # DTC = load('../models/DTC.pkl')
    # lr = load('../models/LogisticRegression.pkl')
    # vectonizer = load('../models/vectonizer')

    # article = data['text']
    # a = tokenizer(article)
    # b = [a]
    # b = vectonizer.transform(b)

    # pred = DTC.predict(b)
    # predicted = { 'predicted': int(pred[0]) }

    return jsonify("Poggers")