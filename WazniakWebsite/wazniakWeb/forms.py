from django import forms
from models import SampleItem


class AddItemForm(forms.ModelForm):
    class Meta:
        model = SampleItem
