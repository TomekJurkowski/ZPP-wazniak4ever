from django.db import models


class SampleItem(models.Model):
    text = models.TextField(blank=True)
    number = models.IntegerField(null=True, blank=True)


    class Meta:
        db_table = 'SampleItem'
