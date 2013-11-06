from models import SampleItem
from django.contrib import admin


class SampleItemAdmin(admin.ModelAdmin):
    list_display = ('text', 'number')

admin.site.register(SampleItem, SampleItemAdmin)