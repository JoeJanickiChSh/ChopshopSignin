#!/usr/bin/env python

import datetime
import os

import flask_excel as excel
from flask import Flask
from flask_bootstrap import Bootstrap5

from .admin import admin
from .auth import auth, login_manager
from .model import Badge, Event, EventType, Role, Subteam, User, db
from .team import team
from .user import user
from .views import qrbp

app = Flask(__name__)

app.secret_key = os.environ.get("FLASK_SECRET_KEY", "1234")

if app.config["DEBUG"]:
    db_name = ':memory:'
else:
    db_name = 'signin.db'
app.config['SQLALCHEMY_DATABASE_URI'] = 'sqlite:///' + db_name
app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = True

excel.init_excel(app)

bootstrap = Bootstrap5(app)
login_manager.init_app(app)

db.init_app(app)
with app.app_context():
    db.create_all()

app.register_blueprint(qrbp)
app.register_blueprint(auth)
app.register_blueprint(admin)
app.register_blueprint(team)
app.register_blueprint(user)

login_manager.login_view = "auth.login"
login_manager.init_app(app)

def init_default_db():

    ADMIN = Role(name="admin", mentor=True, can_display=True, admin=True)
    MENTOR = Role(name="mentor", mentor=True, can_display=True)
    DISPLAY = Role(name="display", can_display=True, autoload=True)
    LEAD = Role(name="lead", can_see_subteam=True)
    STUDENT = Role(name="student")

    TRAINING = EventType(
        name="Training", description="Training Session", autoload=True)
    BUILD = EventType(
        name="Build", description="Build Season", autoload=True)
    FUNDRAISER = EventType(name="Fundraiser", description="Fundraiser")
    COMPETITION = EventType(name="Competition", description="Competition")

    SOFTWARE = Subteam(name="Software")
    MECH = Subteam(name="Mechanical")
    CAD = Subteam(name="CAD")
    MARKETING = Subteam(name="Marketing")

    db.session.add_all([
        ADMIN, MENTOR, DISPLAY, LEAD, STUDENT,
        TRAINING, BUILD, FUNDRAISER, COMPETITION,
        SOFTWARE, MECH, CAD, MARKETING
    ])
    db.session.commit()

    admin_user = User.make("admin", password="1234", role=ADMIN, approved=True)
    display = User.make("display", password="1234",
                        role=DISPLAY, approved=True)
    db.session.add_all([admin_user, display])
    db.session.commit()


if app.config["DEBUG"]:
    with app.app_context():
        init_default_db()

        training = Event(
            name="Training",
            code="5678",
            start=datetime.datetime.fromisoformat("2022-01-01T00:00:00"),
            end=datetime.datetime.fromisoformat("2022-03-01T23:59:59"),
            type_=EventType.query.filter_by(name="Training").one()
        )
        db.session.add_all([training])
        db.session.commit()

        MENTOR = Role.query.filter_by(name="mentor").one()
        STUDENT = Role.query.filter_by(name="student").one()
        SOFTWARE = Subteam.query.filter_by(name="Software").one()

        mentor = User.make("Matt Soucy", password="1234", role=MENTOR)
        student = User.make("Jeff Burke", password="1234",
                            role=STUDENT, subteam=SOFTWARE)
        safe = Badge(name="Safety Certified",
                     icon="cone-striped", color="orange",
                     description="Passed Safety Training")
        db.session.add_all([mentor, student, safe])
        db.session.commit()

        mentor.award_badge(safe.id)
        db.session.commit()
